using Abstractions.Domain.AggregateRoot;
using Movements.Domain.TrainOperations.Enums;
using Movements.Domain.TrainOperations.ValueObjects;
using Movements.Domain.TrainOperations.ValueObjects.TrainOperations;

namespace Movements.Domain.TrainOperations;

/// <summary>
/// Операция над поездом на ж/д пути
/// </summary>
public sealed class TrainOperation : AggregateRoot<TrainOperationId>
{
    /// <summary>
    /// Идентификатор поезда, его номер
    /// </summary>
    public ExternalIdentifier TrainIdentifier { get; private set; }

    /// <summary>
    /// Идентификатор станции отправления, еср
    /// </summary>
    public ExternalIdentifier RailwaySectionFromIdentifier { get; private set; }

    /// <summary>
    /// Идентификатор станции прибытия, еср
    /// </summary>
    public ExternalIdentifier RailwaySectionToIdentifier { get; private set; }

    /// <summary>
    /// Код операции
    /// </summary>
    public TrainOperationCodes Code { get; private set; }

    /// <summary>
    /// Метка времени операции
    /// </summary>
    public DateTime TimeStamp { get; private set; }

    /// <summary>
    /// Только для EF core
    /// </summary>
    private TrainOperation() : base(default)
    {
    }

    private TrainOperation(TrainOperationId id,
                           ExternalIdentifier trainIdentifier,
                           ExternalIdentifier railwaySectionFromIdentifier,
                           ExternalIdentifier railwaySectionToIdentifier,
                           TrainOperationCodes code,
                           DateTime timeStamp) : base(id)
    {
        TrainIdentifier = trainIdentifier;
        RailwaySectionFromIdentifier = railwaySectionFromIdentifier;
        RailwaySectionToIdentifier = railwaySectionToIdentifier;
        Code = code;
        TimeStamp = timeStamp;
    }

    public static TrainOperation Create(ExternalIdentifier trainIdentifier,
                                        ExternalIdentifier railwaySectionFromIdentifier,
                                        ExternalIdentifier railwaySectionToIdentifier,
                                        TrainOperationCodes code,
                                        DateTime timeStamp)
    {
        return new TrainOperation(new TrainOperationId(Guid.CreateVersion7()),
                                  trainIdentifier,
                                  railwaySectionFromIdentifier,
                                  railwaySectionToIdentifier,
                                  code,
                                  timeStamp);
    }
}
