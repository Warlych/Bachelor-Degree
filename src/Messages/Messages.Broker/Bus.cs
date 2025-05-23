using System.Reflection;
using System.Text;
using System.Text.Json;
using Messages.Broker.Abstractions.Bus;
using Messages.Broker.Abstractions.Consumers;
using Messages.Broker.Abstractions.Producers;
using Messages.Broker.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messages.Broker;

public sealed class Bus : IBus
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<Bus> _logger;

    private CancellationTokenSource _cts;

    private readonly List<Task> _consumers = [];

    public Bus(IServiceProvider serviceProvider, ILogger<Bus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _connectionFactory = serviceProvider.GetService<IConnectionFactory>() ?? throw new ArgumentNullException(nameof(_connectionFactory));
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var producer = scope.ServiceProvider.GetService<IProducer<TMessage>>();

        ArgumentNullException.ThrowIfNull(producer, $"producer with message type: {typeof(TMessage).FullName}");

        var attribute = GetQueueAttribute(producer.GetType());

        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(attribute.Queue, attribute.Durable, attribute.Exclusive, attribute.AutoDeleteOnIdle, null);

        var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _logger.LogInformation("Produce message {@message} to queue {attribute}", message, attribute.Queue);

        try
        {
            await channel.BasicPublishAsync(exchange: String.Empty,
                                            routingKey: attribute.Queue,
                                            mandatory: true,
                                            basicProperties: new BasicProperties
                                            {
                                                Persistent = true
                                            },
                                            body: content);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to produce a message {@exection}", ex);

            throw;
        }
    }



    public async Task StartConsumeAsync(CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await using var scope = _serviceProvider.CreateAsyncScope();

        var consumers = scope.ServiceProvider.GetServices<IConsumer>();

        foreach (var consumer in consumers)
        {
            var @interface = consumer.GetType()
                                     .GetInterfaces()
                                     .Where(x => x.IsGenericType)
                                     .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                                     .FirstOrDefault();

            if (@interface is null)
            {
                throw new InvalidOperationException(
                    $"Type '{consumer.GetType().FullName}' does not implement '{typeof(IConsumer<>).FullName}'");
            }

            var attribute = GetQueueAttribute(consumer.GetType());

            var messageType = @interface.GetGenericArguments()[0];

            var method = GetType()
                         .GetMethod(nameof(CreateConsumerTask), BindingFlags.NonPublic | BindingFlags.Instance)
                         .MakeGenericMethod(messageType);

            _consumers.Add(Task.Factory
                               .StartNew(() =>
                                         {
                                             try
                                             {
                                                 return method.Invoke(this,
                                                                      new object[]
                                                                      {
                                                                          attribute,
                                                                          _cts.Token
                                                                      }) as Task;

                                             }
                                             catch (Exception ex)
                                             {
                                                 _logger.LogError("Failed to create consumer worker {@exception}", ex);
                                                 return Task.FromException(ex);
                                             }
                                         },
                                         cancellationToken,
                                         TaskCreationOptions.LongRunning,
                                         TaskScheduler.Default)
                               .Unwrap());
        }
    }

    public Task StopConsumeAsync(CancellationToken cancellationToken = default)
    {
        _cts?.Cancel();

        return Task.WhenAll(_consumers);
    }

    private async Task CreateConsumerTask<TMessage>(QueueAttribute attribute,
                                                    CancellationToken cancellationToken)
        where TMessage : class
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var messageConsumer = scope.ServiceProvider.GetService<IConsumer<TMessage>>();
            
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(attribute.Queue,
                                            attribute.Durable,
                                            attribute.Exclusive,
                                            attribute.AutoDeleteOnIdle,
                                            null);

            _logger.LogInformation("Started consuming messages from queue {queue}", attribute.Queue);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                try
                {
                    var message = JsonSerializer.Deserialize<TMessage>(Encoding.UTF8.GetString(eventArgs.Body.Span));

                    if (message != null)
                    {
                        _logger.LogInformation("Received message {@message} from queue {queue}", message, attribute.Queue);

                        await messageConsumer.ConsumeAsync(message, cancellationToken);

                        await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                    }
                    else
                    {
                        _logger.LogWarning("Received null message from queue {queue}", attribute.Queue);

                        await channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue {queue}", attribute.Queue);

                    await channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                }
            };

            var consumerTag = await channel.BasicConsumeAsync(
                                  attribute.Queue,
                                  false,
                                  consumer);

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await channel.BasicCancelAsync(consumerTag);

                _logger.LogInformation("Stopped consuming messages from queue {queue}", attribute.Queue);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in consumer for queue {queue}", attribute.Queue);
            throw;
        }
    }

    private static QueueAttribute GetQueueAttribute(Type type)
    {
        var attribute = type.GetCustomAttribute<QueueAttribute>();

        ArgumentNullException.ThrowIfNull(attribute, nameof(attribute));

        return attribute;
    }
}