using RabbitMQ.Client;

namespace Messages.Broker.Abstractions.Producers;

public interface IProducer<TMessage> : IProducer
    where TMessage : class
{
    public ValueTask PublishAsync(TMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Marker interface for producer
/// </summary>
public interface IProducer;