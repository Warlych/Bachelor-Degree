using System.Linq.Expressions;
using Abstractions.Domain.AggregateRoot;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

namespace RailwaySections.Domain.RailwaySections.Repositories;

public interface IRailwaySectionRepository : IAggregateRootRepository<RailwaySection, RailwaySectionId>
{
    Task<IEnumerable<RailwaySection>> GetAsync(Expression<Func<RailwaySection, bool>> predicate,
                                               int pageNumber,
                                               int pageSize,
                                               CancellationToken cancellationToken = default);
    Task<(int Length, IEnumerable<RailwaySection> RailwaySections)> GetRailwaySectionLengthAsync(RailwaySection from,
                                                                                                 RailwaySection to,
                                                                                                 CancellationToken cancellationToken = default);

    Task BuildGraphAsync(CancellationToken cancellationToken = default);
    Task DropGraphIfExistsAsync(CancellationToken cancellationToken = default);
}
