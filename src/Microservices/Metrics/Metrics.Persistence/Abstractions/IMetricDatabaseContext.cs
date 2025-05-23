using Abstractions.Persistence;
using Metrics.Domain.Metrics;
using Metrics.Domain.Metrics.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Metrics.Persistence.Abstractions;

public interface IMetricDatabaseContext : IDatabaseContext
{
    ChangeTracker ChangeTracker { get; }
    DatabaseFacade Database { get; }
    DbSet<Metric> Metrics { get; set; }
    DbSet<RailwaySection> RailwaySections { get; set; }
    DbSet<Train> Trains { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
