using Abstractions.Domain.AggregateRoot;
using Metrics.Domain.Metrics.ValueObjects;
using Metrics.Domain.Metrics.ValueObjects.Trains;

namespace Metrics.Domain.Metrics.Entities;

public sealed class Train : Entity<TrainId>
{
    /// <summary>
    /// Номер поезда
    /// </summary>
    public ExternalIdentifier ExternalIdentifier { get; set; }

    /// <summary>
    /// Только для EF core
    /// </summary>
    private Train() : base(default)
    {
    }

    public Train(TrainId id, ExternalIdentifier externalIdentifier) : base(id)
    {
        ExternalIdentifier = externalIdentifier;
    }
}
