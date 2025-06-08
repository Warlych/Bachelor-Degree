using Abstractions.Persistence;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mediator;
using Metrics.Application.Handlers.Queries;
using Metrics.Domain.Metrics;
using Metrics.Domain.Metrics.Repositories;
using Metrics.Domain.Metrics.ValueObjects;
using Microsoft.Extensions.Logging;
using Movements.Contracts.Grpc.Impl.Movements;
using RailwaySections.Contracts.Grpc.Impl.RailwaySections;
using Trains.Contracts.Grpc.Impl.Trains;
using DateTimeRange = Metrics.Domain.Metrics.ValueObjects.Metrics.DateTimeRange;
using RailwaySection = Movements.Contracts.Grpc.Impl.Movements.RailwaySection;
using RailwaySectionMetrics = Metrics.Domain.Metrics.ValueObjects.Metrics.RailwaySectionMetrics;
using Train = Metrics.Domain.Metrics.Entities.Train;

namespace Metrics.Application.Handlers.Commands.Create;

public record CreateMetricCommand(ExternalIdentifier RailwaySectionFrom, ExternalIdentifier RailwaySectionTo, DateTime From, DateTime To)
    : ICommand<MetricDto>;

public sealed class CreateMetricCommandHandler : ICommandHandler<CreateMetricCommand, MetricDto>
{
    private readonly MovementsMicroservice.MovementsMicroserviceClient _movementsMicroserviceClient;
    private readonly RailwaySectionsMicroservice.RailwaySectionsMicroserviceClient _railwaySectionsMicroserviceClient;
    private readonly TrainsMicroservice.TrainsMicroserviceClient _trainsMicroserviceClient;

    private readonly IMetricsRepository _metricsRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateMetricCommandHandler> _logger;

    public CreateMetricCommandHandler(MovementsMicroservice.MovementsMicroserviceClient movementsMicroserviceClient,
                                      RailwaySectionsMicroservice.RailwaySectionsMicroserviceClient railwaySectionsMicroserviceClient,
                                      TrainsMicroservice.TrainsMicroserviceClient trainsMicroserviceClient,
                                      IMetricsRepository metricsRepository,
                                      IUnitOfWork unitOfWork,
                                      ILogger<CreateMetricCommandHandler> logger)
    {
        _movementsMicroserviceClient = movementsMicroserviceClient;
        _railwaySectionsMicroserviceClient = railwaySectionsMicroserviceClient;
        _trainsMicroserviceClient = trainsMicroserviceClient;
        _metricsRepository = metricsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<MetricDto> Handle(CreateMetricCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var trainDurations = await _movementsMicroserviceClient
                                     .GetTrainsTravellingTimeOnRailwaySectionAsync(new GetTrainsTravellingTimeOnRailwaySectionRequest
                                                                                   {
                                                                                       RailwaySectionFrom = new RailwaySection
                                                                                       {
                                                                                           UnifiedNetworkMarking =
                                                                                               command.RailwaySectionFrom.ToString()
                                                                                       },
                                                                                       RailwaySectionTo = new RailwaySection
                                                                                       {
                                                                                           UnifiedNetworkMarking = command.RailwaySectionTo.ToString()
                                                                                       },
                                                                                       From = Timestamp.FromDateTime(command.From),
                                                                                       To = Timestamp.FromDateTime(command.To)
                                                                                   },
                                                                                   new CallOptions(cancellationToken: cancellationToken));

            if (!trainDurations.Success)
            {
                throw new InvalidOperationException(
                    $"Failed to get trains travelling time on railway section: {trainDurations.Error.Title} {trainDurations.Error.Message}");
            }
            
            if (!trainDurations.TrainMovementDurations.TrainMovementDurations_.Any())
            {
                throw new InvalidOperationException($"No train movement durations for railway section from: {command.RailwaySectionFrom} to: {command.RailwaySectionTo}");
            }
                
            var railwaySectionLength = await _railwaySectionsMicroserviceClient
                                           .GetRailwaySectionLengthAsync(new GetRailwaySectionLengthRequest
                                                                         {
                                                                             From = new RailwaySectionParameters
                                                                             {
                                                                                 RailwayCode = "not needed",
                                                                                 UnifiedNetworkMarking = command.RailwaySectionFrom.ToString()
                                                                             },
                                                                             To = new RailwaySectionParameters
                                                                             {
                                                                                 RailwayCode = "not needed",
                                                                                 UnifiedNetworkMarking = command.RailwaySectionTo.ToString()
                                                                             }
                                                                         },
                                                                         new CallOptions(
                                                                             cancellationToken: cancellationToken));

            if (!railwaySectionLength.Success)
            {
                throw new InvalidOperationException(
                    $"Failed to get railway section length: {railwaySectionLength.Error.Title} {railwaySectionLength.Error.Message}");
            }

            var trains = await _trainsMicroserviceClient
                             .GetTrainsAsync(new GetTrainsRequest
                                             {
                                                 ExternalIdentifier =
                                                 {
                                                     trainDurations.TrainMovementDurations.TrainMovementDurations_
                                                                   .Select(x => x.Train.Number)
                                                 }
                                             },
                                             new CallOptions(cancellationToken: cancellationToken));

            if (!trains.Succes)
            {
                throw new InvalidOperationException($"Failed to get trains {trains.Error.Title} {trains.Error.Message}");
            }

            if (!trains.Trains.Trains_.Any())
            {
                throw new InvalidOperationException("No trains found");
            }
            
            var metrics = CalculateMetrics(railwaySectionLength, trainDurations, trains);

            var savedRailwaySectionFrom = await _metricsRepository.GetRailwaySectionAsync(x => x.ExternalIdentifier == command.RailwaySectionFrom,
                                                                                          cancellationToken);

            if (savedRailwaySectionFrom is null)
            {
                savedRailwaySectionFrom = Metric.CreateSection(command.RailwaySectionFrom);
            }

            var savedRailwaySectionTo = await _metricsRepository.GetRailwaySectionAsync(x => x.ExternalIdentifier == command.RailwaySectionTo,
                                                                                        cancellationToken);
            
            if (savedRailwaySectionTo is null)
            {
                savedRailwaySectionTo = Metric.CreateSection(command.RailwaySectionTo);
            }
            
            var trainIdentifiers = trains.Trains.Trains_.Select(x => x.ExternalIdentifier).ToList();
            
            var savedTrains = await _metricsRepository.GetTrainsAsync(x => trainIdentifiers.Contains(x.ExternalIdentifier.Value),
                                                                      cancellationToken);

            List<Train> metricTrains = [];

            foreach (var train in trains.Trains.Trains_)
            {
                var saved = savedTrains.FirstOrDefault(x => x.ExternalIdentifier.Value == train.ExternalIdentifier);

                if (saved is not null)
                {
                    metricTrains.Add(saved);

                    continue;
                }

                metricTrains.Add(Metric.CreateTrain(new ExternalIdentifier(train.ExternalIdentifier)));
            }

            var metric = Metric.Create(savedRailwaySectionFrom,
                                       savedRailwaySectionTo,
                                       new DateTimeRange(command.From, command.To),
                                       metrics,
                                       metricTrains);

            await _metricsRepository.AddAsync(metric, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

            return MetricDto.Create(metric);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _logger.LogError(ex, "Failed to create metric");

            throw;
        }
    }

    private RailwaySectionMetrics CalculateMetrics(GetRailwaySectionLengthResponse railwaySectionLength,
                                                   GetTrainsTravellingTimeOnRailwaySectionResponse trainDurations,
                                                   GetTrainsResponse trains)
    {
        var totalGrossWeight = 0d;
        var totalNetWeight = 0d;
        var totalTrainLength = 0d;
        var totalTrains = 0;
        var totalSectionLengthInKm = railwaySectionLength.RailwaySectionLength.Length / 1000d;
        var totalTrainKm = 0d;

        var totalTime = TimeSpan.Zero;

        foreach (var trainMovement in trainDurations.TrainMovementDurations.TrainMovementDurations_)
        {
            var trainNumber = trainMovement.Train.Number;
            var train = trains.Trains.Trains_.FirstOrDefault(t => t.ExternalIdentifier == trainNumber);

            if (train != null)
            {
                totalGrossWeight += train.Parameters.GrossWeight;
                totalNetWeight += train.Parameters.NetWeight;

                totalTrainLength += train.Parameters.Length;

                totalTrains++;

                totalTrainKm += totalSectionLengthInKm;

                var travelTime = trainMovement.EndTime.ToDateTime() - trainMovement.StartTime.ToDateTime();

                totalTime += travelTime;
            }
        }

        var averageNetWeight = totalTrains > 0 ? (totalNetWeight * 1000d) / totalTrainKm : 0d;
        var averageGrossWeight = totalTrains > 0 ? (totalGrossWeight * 1000d) / totalTrainKm : 0d;
        var averageTrainLength = totalTrains > 0 ? totalTrainLength / totalTrains : 0d;

        var sectionSpeed = 0d;

        if (totalTime.TotalHours > 0)
        {
            sectionSpeed = totalTrainKm
                           / (totalTime.TotalHours * railwaySectionLength.RailwaySectionLength.PercentageMainSections
                              + totalTime.TotalHours * railwaySectionLength.RailwaySectionLength.PercentageAuxiliarySections);
        }

        var technicalSpeed = 0d;

        if (totalTime.TotalHours > 0)
        {
            technicalSpeed = totalTrainKm
                             / (totalTime.TotalHours * railwaySectionLength.RailwaySectionLength.PercentageMainSections
                                + totalTime.TotalHours * railwaySectionLength.RailwaySectionLength.PercentageTechinalStations);
        }

        var routeSpeed = 0d;

        if (totalTime.TotalHours > 0)
        {
            routeSpeed = totalTrainKm / totalTime.TotalHours;
        }

        return new RailwaySectionMetrics(averageNetWeight,
                                         averageGrossWeight,
                                         averageTrainLength,
                                         sectionSpeed,
                                         technicalSpeed,
                                         routeSpeed);
    }
}
