using Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using Trains.Domain.Trains.Repositories;
using Trains.Domain.Trains.ValueObjects;
using Trains.Domain.Trains.ValueObjects.Trains;

namespace Trains.Application.Handlers.Queries.GetTrain;

public sealed record GetTrainQuery(ExternalIdentifier Identifier) : IQuery<TrainDto>;

public sealed class GetTrainQueryHandler : IQueryHandler<GetTrainQuery, TrainDto>
{
    private readonly ITrainRepository _trainRepository;
    private readonly ILogger<GetTrainQueryHandler> _logger;

    public GetTrainQueryHandler(ITrainRepository trainRepository,
                                ILogger<GetTrainQueryHandler> logger)
    {
        _trainRepository = trainRepository;
        _logger = logger;
    }

    public async ValueTask<TrainDto> Handle(GetTrainQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var train = await _trainRepository.GetAsync(x => x.ExternalIdentifier == query.Identifier, cancellationToken);
            
            ArgumentNullException.ThrowIfNull(train, nameof(train));
            
            _logger.LogInformation($"Retrieved train with ID: {train.Id}");

            return new TrainDto(train.Id.Identity,
                                train.ExternalIdentifier.ToString(),
                                new TrainParameters(train.Parameters.NumberOfWagons,
                                                    train.Parameters.GrossWeight,
                                                    train.Parameters.NetWeight,
                                                    train.Parameters.Length));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get train");
            
            throw;
        }
    }
}