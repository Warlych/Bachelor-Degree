using System.Linq.Expressions;
using Abstractions.Domain.AggregateRoot;
using Metrics.Domain.Metrics.Entities;
using Metrics.Domain.Metrics.ValueObjects.Metrics;
using Metrics.Domain.Metrics.ValueObjects.RailwaySections;

namespace Metrics.Domain.Metrics.Repositories;

public interface IMetricsRepository : IAggregateRootRepository<Metric, MetricId>
{
    Task<RailwaySection?> GetRailwaySectionAsync(Expression<Func<RailwaySection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Train>> GetTrainsAsync(Expression<Func<Train, bool>> predicate, CancellationToken cancellationToken = default);
}