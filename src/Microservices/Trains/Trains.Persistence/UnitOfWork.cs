using Abstractions.Persistence;
using Trains.Persistence.Abstractions;

namespace Trains.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ITrainDatabaseContext _context;

    public UnitOfWork(ITrainDatabaseContext context)
    {
        _context = context;
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.BeginTransactionAsync(cancellationToken);
    }
    
    public async Task CommitTransactionAsync(bool autoSaveEnable = true, CancellationToken cancellationToken = default)
    {
        if (autoSaveEnable)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
