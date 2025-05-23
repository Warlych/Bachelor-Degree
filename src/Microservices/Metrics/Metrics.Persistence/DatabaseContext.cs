using System.Reflection;
using Metrics.Domain.Metrics;
using Metrics.Domain.Metrics.Entities;
using Metrics.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Metrics.Persistence;

public sealed class DatabaseContext : DbContext, IMetricDatabaseContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
    
    public DbSet<Metric> Metrics { get; set; }
    public DbSet<RailwaySection> RailwaySections { get; set; }
    public DbSet<Train> Trains { get; set; }
}
