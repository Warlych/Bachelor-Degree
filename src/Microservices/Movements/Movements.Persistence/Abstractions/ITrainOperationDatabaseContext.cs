using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Movements.Domain.TrainOperations;

namespace Movements.Persistence.Abstractions;

public interface ITrainOperationDatabaseContext : IDatabaseContext
{
    ChangeTracker ChangeTracker { get; }
    DatabaseFacade Database { get; }
    DbSet<TrainOperation> TrainOperations { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}