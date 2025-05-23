namespace Abstractions.Domain.AggregateRoot.Events;

/// <summary>
/// Defines a handler for the domain event
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IDomainEventHandler<in TRequest> where TRequest : IDomainEvent;