using Abstractions.Domain.AggregateRoot;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

namespace RailwaySections.Domain.RailwaySections.Repositories;

public interface IRailwaySectionRepository : IAggregateRootRepository<RailwaySection, RailwaySectionId>
{
    Task<(int Length, IEnumerable<RailwaySection> RailwaySections)> GetRailwaySectionLengthAsync(RailwaySection from,
                                                                                                 RailwaySection to,
                                                                                                 CancellationToken cancellationToken);

    Task BuildGraphAsync(CancellationToken cancellationToken = default);
    Task DropGraphIfExistsAsync(CancellationToken cancellationToken = default);
}
