using Abstractions.Domain.AggregateRoot;
using Metrics.Domain.Metrics.ValueObjects;
using Metrics.Domain.Metrics.ValueObjects.RailwaySections;

namespace Metrics.Domain.Metrics.Entities;

public sealed class RailwaySection : Entity<RailwaySectionId>
{
    /// <summary>
    /// ЕСР
    /// </summary>
    public ExternalIdentifier ExternalIdentifier { get; set; }
    
    /// <summary>
    /// Только для EF core
    /// </summary>
    private RailwaySection() : base(default)
    {
    }

    public RailwaySection(RailwaySectionId id, ExternalIdentifier externalIdentifier) : base(id)
    {
        ExternalIdentifier = externalIdentifier;
    }
}
