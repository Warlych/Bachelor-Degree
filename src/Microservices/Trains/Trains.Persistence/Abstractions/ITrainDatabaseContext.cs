using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Trains.Domain.Trains;

namespace Trains.Persistence.Abstractions;

public interface ITrainDatabaseContext : IDatabaseContext
{
    ChangeTracker ChangeTracker { get; }
    DatabaseFacade Database { get; }
    DbSet<Train> Trains { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
