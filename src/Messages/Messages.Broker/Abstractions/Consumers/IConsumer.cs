namespace Messages.Broker.Abstractions.Consumers;

public interface IConsumer<TMessage> : IConsumer
    where TMessage : class
{
    ValueTask ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Marker interface for consumer
/// </summary>
public interface IConsumer;
