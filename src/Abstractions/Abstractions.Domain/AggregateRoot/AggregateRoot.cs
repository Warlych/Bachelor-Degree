using Abstractions.Domain.AggregateRoot.Events;

namespace Abstractions.Domain.AggregateRoot;

/// <summary>
/// Abstract class for an aggregate
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();
    
    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected void AddEvent(IDomainEvent @event)
    {
        _events.Add(@event);
    }
}
