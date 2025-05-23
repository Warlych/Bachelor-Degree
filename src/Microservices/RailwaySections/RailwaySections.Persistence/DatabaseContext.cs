using Neo4j.Driver;
using RailwaySections.Persistence.Abstractions;

namespace RailwaySections.Persistence;

public sealed class DatabaseContext : IRailwaySectionDatabaseContext
{
    private readonly IDriver _driver;

    public DatabaseContext(IDriver driver)
    {
        _driver = driver;
    }

    public IAsyncSession AsyncSession()
    {
        return _driver.AsyncSession();
    }
}
