using System.Linq.Expressions;
using Abstractions.Domain.AggregateRoot;
using Trains.Domain.Trains.ValueObjects.Trains;

namespace Trains.Domain.Trains.Repositories;

public interface ITrainRepository : IAggregateRootRepository<Train, TrainId>
{
    Task<IEnumerable<Train>> GetAllAsync(Expression<Func<Train, bool>> predicate, CancellationToken cancellationToken = default);
}
