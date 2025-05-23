using Abstractions.Persistence;
using Metrics.Persistence.Abstractions;

namespace Metrics.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IMetricDatabaseContext _context;

    public UnitOfWork(IMetricDatabaseContext context)
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
