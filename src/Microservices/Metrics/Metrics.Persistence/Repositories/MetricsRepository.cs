using System.Linq.Expressions;
using Metrics.Domain.Metrics;
using Metrics.Domain.Metrics.Entities;
using Metrics.Domain.Metrics.Repositories;
using Metrics.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Metrics.Persistence.Repositories;

public sealed class MetricsRepository : IMetricsRepository
{
    private readonly IMetricDatabaseContext _context;

    public MetricsRepository(IMetricDatabaseContext context)
    {
        _context = context;
    }

    public Task<Metric?> GetAsync(Expression<Func<Metric, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _context.Metrics.Include(x => x.From)
                               .Include(x => x.To)
                               .Include(x => x.Trains)
                               .Where(predicate ?? (_ => true))
                               .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Metric aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Metrics.AddAsync(aggregate, cancellationToken);
    }

    public Task DeleteAsync(Metric aggregate, CancellationToken cancellationToken = default)
    {
        _context.Metrics.Remove(aggregate);

        return Task.CompletedTask;
    }

    public Task<RailwaySection?> GetRailwaySectionAsync(Expression<Func<RailwaySection, bool>> predicate,
                                                        CancellationToken cancellationToken = default)
    {
        return _context.RailwaySections.Where(predicate ?? (_ => true)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Train>> GetTrainsAsync(Expression<Func<Train, bool>> predicate, CancellationToken cancellationToken = default)
    {
       return await _context.Trains.Where(predicate ?? (_ => true)).ToListAsync(cancellationToken);
    }
}
