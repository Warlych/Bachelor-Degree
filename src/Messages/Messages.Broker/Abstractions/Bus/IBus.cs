using RabbitMQ.Client;

namespace Messages.Broker.Abstractions.Bus;

public interface IBus
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class; 
    
    Task StartConsumeAsync(CancellationToken cancellationToken = default);
    Task StopConsumeAsync(CancellationToken cancellationToken = default);
}
