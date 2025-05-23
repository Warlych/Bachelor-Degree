using Mediator;
using Microsoft.Extensions.Logging;
using Movements.Domain.TrainOperations;
using Movements.Domain.TrainOperations.Enums;
using Movements.Domain.TrainOperations.Repositories;
using Movements.Domain.TrainOperations.ValueObjects;

namespace Movements.Application.Handlers.Queries.GetTrainsTravellingTimeOnRailwaySection;

public record GetTrainsTravellingTimeOnRailwaySectionQuery(ExternalIdentifier RailwaySectionFrom,
                                                           ExternalIdentifier RailwaySectionTo,
                                                           DateTime From,
                                                           DateTime To) : IQuery<IEnumerable<TrainMovementDuration>>;

public sealed class GetTrainsTravellingTimeOnRailwaySectionQueryHandler
    : IQueryHandler<GetTrainsTravellingTimeOnRailwaySectionQuery, IEnumerable<TrainMovementDuration>>
{
    private readonly ITrainOperationRepository _trainOperationRepository;
    private readonly ILogger<GetTrainsTravellingTimeOnRailwaySectionQueryHandler> _logger;

    public GetTrainsTravellingTimeOnRailwaySectionQueryHandler(ITrainOperationRepository trainOperationRepository,
                                                               ILogger<GetTrainsTravellingTimeOnRailwaySectionQueryHandler> logger)
    {
        _trainOperationRepository = trainOperationRepository;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<TrainMovementDuration>> Handle(GetTrainsTravellingTimeOnRailwaySectionQuery query,
                                                                      CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing train travelling time query for railway section from {RailwaySectionFrom} to {RailwaySectionTo} between {From} and {To}",
            query.RailwaySectionFrom,
            query.RailwaySectionTo,
            query.From,
            query.To);

        var operations = await _trainOperationRepository.GetAllAsync(
                             x => x.TimeStamp >= query.From
                                  && x.TimeStamp <= query.To
                                  && x.RailwaySectionFromIdentifier == query.RailwaySectionFrom
                                  && x.RailwaySectionToIdentifier == query.RailwaySectionTo,
                             cancellationToken);

        _logger.LogDebug("Retrieved {OperationsCount} train operations for the specified criteria", operations.Count());

        var grouped = operations.GroupBy(x => new
        {
            TrainNumber = x.TrainIdentifier.Value,
            From = x.RailwaySectionFromIdentifier.Value,
            To = x.RailwaySectionToIdentifier.Value
        });

        var result = new List<TrainMovementDuration>();
        var groupCount = 0;

        foreach (var group in grouped)
        {
            groupCount++;
            _logger.LogDebug("Processing group {GroupNumber}: Train {TrainNumber} from {From} to {To} with {OperationsCount} operations",
                             groupCount,
                             group.Key.TrainNumber,
                             group.Key.From,
                             group.Key.To,
                             group.Count());

            var allOperations = group.OrderBy(x => x.TimeStamp).ToList();

            var movements = ExtractMovements(allOperations, group.Key.TrainNumber);

            _logger.LogDebug("Extracted {MovementsCount} movements for train {TrainNumber}", movements.Count, group.Key.TrainNumber);

            foreach (var movement in movements)
            {
                result.Add(new TrainMovementDuration(
                               new Train(group.Key.TrainNumber),
                               new RailwaySection(group.Key.From),
                               new RailwaySection(group.Key.To),
                               movement.StartTime,
                               movement.EndTime));
            }
        }

        _logger.LogInformation("Successfully processed {ResultCount} train movement durations from {GroupsCount} train groups",
                               result.Count,
                               groupCount);

        return result;
    }

    private List<(DateTime StartTime, DateTime EndTime)> ExtractMovements(List<TrainOperation> operations, string trainNumber)
    {
        var movements = new List<(DateTime StartTime, DateTime EndTime)>();
        DateTime? currentMovementStart = null;

        _logger.LogTrace("Starting movement extraction for train {TrainNumber} with {OperationsCount} operations",
                         trainNumber,
                         operations.Count);

        foreach (var operation in operations)
        {
            _logger.LogTrace("Processing operation {OperationCode} at {TimeStamp} for train {TrainNumber}",
                             operation.Code,
                             operation.TimeStamp,
                             trainNumber);

            switch (operation.Code)
            {
                case TrainOperationCodes.StartOfMovement:
                    if (currentMovementStart.HasValue)
                    {
                        _logger.LogWarning(
                            "Detected start of movement without completing previous movement for train {TrainNumber} at {TimeStamp}. Previous start was at {PreviousStart}",
                            trainNumber,
                            operation.TimeStamp,
                            currentMovementStart.Value);
                    }

                    currentMovementStart = operation.TimeStamp;
                    _logger.LogTrace("Movement started for train {TrainNumber} at {TimeStamp}", trainNumber, operation.TimeStamp);
                    break;

                case TrainOperationCodes.EndOfMovement:
                    if (currentMovementStart.HasValue)
                    {
                        if (operation.TimeStamp > currentMovementStart.Value)
                        {
                            var duration = operation.TimeStamp - currentMovementStart.Value;
                            movements.Add((currentMovementStart.Value, operation.TimeStamp));
                            _logger.LogTrace("Movement completed for train {TrainNumber}. Duration: {Duration} (from {StartTime} to {EndTime})",
                                             trainNumber,
                                             duration,
                                             currentMovementStart.Value,
                                             operation.TimeStamp);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Invalid movement duration for train {TrainNumber}: end time {EndTime} is not after start time {StartTime}",
                                trainNumber,
                                operation.TimeStamp,
                                currentMovementStart.Value);
                        }

                        currentMovementStart = null;
                    }
                    else
                    {
                        _logger.LogWarning("Detected end of movement without corresponding start for train {TrainNumber} at {TimeStamp}",
                                           trainNumber,
                                           operation.TimeStamp);
                    }

                    break;

                case TrainOperationCodes.Arrival:
                case TrainOperationCodes.ArrivalToStationArea:
                    if (currentMovementStart.HasValue && operation.TimeStamp > currentMovementStart.Value)
                    {
                        var duration = operation.TimeStamp - currentMovementStart.Value;
                        movements.Add((currentMovementStart.Value, operation.TimeStamp));
                        _logger.LogTrace(
                            "Movement completed by arrival for train {TrainNumber}. Duration: {Duration} (from {StartTime} to {EndTime})",
                            trainNumber,
                            duration,
                            currentMovementStart.Value,
                            operation.TimeStamp);

                        currentMovementStart = null;
                    }

                    break;

                case TrainOperationCodes.Departure:
                case TrainOperationCodes.DepartureFromStationArea:
                    if (!currentMovementStart.HasValue)
                    {
                        currentMovementStart = operation.TimeStamp;
                        _logger.LogTrace("Movement started by departure for train {TrainNumber} at {TimeStamp}", trainNumber, operation.TimeStamp);
                    }

                    break;

                case TrainOperationCodes.PassThrough:
                    if (!currentMovementStart.HasValue)
                    {
                        movements.Add((operation.TimeStamp, operation.TimeStamp));
                        _logger.LogTrace("Pass-through movement recorded for train {TrainNumber} at {TimeStamp}", trainNumber, operation.TimeStamp);
                    }

                    break;
            }
        }

        if (currentMovementStart.HasValue)
        {
            _logger.LogWarning("Uncompleted movement detected for train {TrainNumber}. Movement started at {StartTime} but no end operation found",
                               trainNumber,
                               currentMovementStart.Value);
        }

        _logger.LogDebug("Movement extraction completed for train {TrainNumber}. Total movements found: {MovementsCount}",
                         trainNumber,
                         movements.Count);

        return movements;
    }
}


public record TrainMovementDuration(Train Train, RailwaySection From, RailwaySection To, DateTime Start, DateTime End);
