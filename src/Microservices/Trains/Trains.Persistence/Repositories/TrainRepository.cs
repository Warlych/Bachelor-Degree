using System.Linq.Expressions;
using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Trains.Domain.Trains;
using Trains.Domain.Trains.Repositories;
using Trains.Persistence.Abstractions;

namespace Trains.Persistence.Repositories;

public sealed class TrainRepository : ITrainRepository
{
    private readonly ITrainDatabaseContext _context;

    public TrainRepository(ITrainDatabaseContext context)
    {
        _context = context;
    }

    public Task<Train?> GetAsync(Expression<Func<Train, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _context.Trains.Where(predicate ?? (_ => true)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Train aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Trains.AddAsync(aggregate, cancellationToken);
    }

    public Task DeleteAsync(Train aggregate, CancellationToken cancellationToken = default)
    {
        _context.Trains.Remove(aggregate);
        
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Train>> GetAllAsync(Expression<Func<Train, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Trains.Where(predicate ?? (_ => true)).ToListAsync(cancellationToken);
    }
}
