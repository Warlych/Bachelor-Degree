using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Movements.Domain.TrainOperations;
using Movements.Persistence.Abstractions;

namespace Movements.Persistence;

public sealed class DatabaseContext : DbContext, ITrainOperationDatabaseContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
    
    public DbSet<TrainOperation> TrainOperations { get; set; }
}
