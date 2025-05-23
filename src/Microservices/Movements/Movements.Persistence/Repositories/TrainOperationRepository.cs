using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Movements.Domain.TrainOperations;
using Movements.Domain.TrainOperations.Repositories;
using Movements.Persistence.Abstractions;

namespace Movements.Persistence.Repositories;

public sealed class TrainOperationRepository : ITrainOperationRepository
{
    private readonly ITrainOperationDatabaseContext _context;

    public TrainOperationRepository(ITrainOperationDatabaseContext context)
    {
        _context = context;
    }

    public Task<TrainOperation?> GetAsync(Expression<Func<TrainOperation, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _context.TrainOperations.Where(predicate ?? (_ => true)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(TrainOperation aggregate, CancellationToken cancellationToken = default)
    {
        await _context.TrainOperations.AddAsync(aggregate, cancellationToken);
    }

    public Task DeleteAsync(TrainOperation aggregate, CancellationToken cancellationToken = default)
    {
        _context.TrainOperations.Remove(aggregate);

        return Task.CompletedTask;
    }

    public IAsyncEnumerable<TrainOperation> GetAllAsAsyncEnumerableAsync(Expression<Func<TrainOperation, bool>> predicate,
                                                                         CancellationToken cancellationToken = default)
    {
        return _context.TrainOperations.Where(predicate ?? (_ => true)).AsAsyncEnumerable();
    }

    public async Task<IEnumerable<TrainOperation>> GetAllAsync(Expression<Func<TrainOperation, bool>> predicate,
                                                               CancellationToken cancellationToken = default)
    {
        return await _context.TrainOperations.Where(predicate ?? (_ => true)).ToListAsync(cancellationToken);
    }
}
