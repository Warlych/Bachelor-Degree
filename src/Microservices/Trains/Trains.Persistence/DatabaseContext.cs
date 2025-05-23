using System.Reflection;
using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Trains.Domain.Trains;
using Trains.Persistence.Abstractions;

namespace Trains.Persistence;

public sealed class DatabaseContext : DbContext, ITrainDatabaseContext
{
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public DbSet<Train> Trains { get; set; }
}
