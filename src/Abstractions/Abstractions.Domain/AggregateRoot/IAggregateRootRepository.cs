using System.Linq.Expressions;

namespace Abstractions.Domain.AggregateRoot;

public interface IAggregateRootRepository<T, TId> where T : AggregateRoot<TId>
{
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T aggregate, CancellationToken cancellationToken = default);
    Task DeleteAsync(T aggregate, CancellationToken cancellationToken = default);
}
