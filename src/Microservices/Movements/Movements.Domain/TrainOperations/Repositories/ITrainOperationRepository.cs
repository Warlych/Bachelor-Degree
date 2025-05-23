using System.Linq.Expressions;
using Abstractions.Domain.AggregateRoot;
using Movements.Domain.TrainOperations.ValueObjects.TrainOperations;

namespace Movements.Domain.TrainOperations.Repositories;

public interface ITrainOperationRepository : IAggregateRootRepository<TrainOperation, TrainOperationId>
{
    IAsyncEnumerable<TrainOperation> GetAllAsAsyncEnumerableAsync(Expression<Func<TrainOperation, bool>> predicate, CancellationToken cancellationToken = default); 
    Task<IEnumerable<TrainOperation>> GetAllAsync(Expression<Func<TrainOperation, bool>> predicate, CancellationToken cancellationToken = default);
}
