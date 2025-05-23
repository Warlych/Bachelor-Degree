using Abstractions.Domain.AggregateRoot;
using Trains.Domain.Trains.ValueObjects;
using Trains.Domain.Trains.ValueObjects.Trains;

namespace Trains.Domain.Trains;

/// <summary>
/// Объект поезда
/// </summary>
public sealed class Train : AggregateRoot<TrainId>
{
    /// <summary>
    /// Идентификатор сущности в других системах
    /// </summary>
    public ExternalIdentifier ExternalIdentifier { get; private set; }

    /// <summary>
    /// Параметры поезда (вес в брутто/нетто, длина)
    /// </summary>
    public TrainParameters Parameters { get; private set; }

    /// <summary>
    /// Только для EF core
    /// </summary>
    private Train() : base(default)
    {
    }
    
    public Train(TrainId id,
                 ExternalIdentifier externalIdentifier,
                 TrainParameters parameters)
        : base(id)
    {
        ExternalIdentifier = externalIdentifier;
        Parameters = parameters;
    }
}
