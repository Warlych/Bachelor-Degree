using Messages.Broker.Abstractions.Bus;
using Messages.Broker.Abstractions.Producers;

namespace Messages.Broker.Producers;

public abstract class Producer<TMessage> : IProducer<TMessage> 
    where TMessage : class
{
    private readonly IBus _bus;
    
    public Producer(IBus bus)
    {
        _bus = bus;
    }

    public async virtual ValueTask PublishAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await _bus.PublishAsync(message, cancellationToken);
    }
}
