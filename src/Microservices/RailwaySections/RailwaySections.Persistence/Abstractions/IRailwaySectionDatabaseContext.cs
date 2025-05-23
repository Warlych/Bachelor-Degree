using Abstractions.Persistence;
using Neo4j.Driver;

namespace RailwaySections.Persistence.Abstractions;

public interface IRailwaySectionDatabaseContext : IDatabaseContext
{
    IAsyncSession AsyncSession();
}
