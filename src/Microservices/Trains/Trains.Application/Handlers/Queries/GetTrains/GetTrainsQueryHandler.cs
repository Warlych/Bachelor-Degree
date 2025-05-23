using Mediator;
using Microsoft.Extensions.Logging;
using Trains.Domain.Trains.Repositories;
using Trains.Domain.Trains.ValueObjects;

namespace Trains.Application.Handlers.Queries.GetTrains;

public record GetTrainsQuery(IEnumerable<ExternalIdentifier> Identifiers) 
    : IQuery<IEnumerable<TrainDto>>;

public sealed class GetTrainsQueryHandler : IQueryHandler<GetTrainsQuery, IEnumerable<TrainDto>>
{
    private readonly ITrainRepository _trainRepository;
    private readonly ILogger<GetTrainsQueryHandler> _logger;

    public GetTrainsQueryHandler(ITrainRepository trainRepository, ILogger<GetTrainsQueryHandler> logger)
    {
        _trainRepository = trainRepository;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<TrainDto>> Handle(GetTrainsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var identifiers = query.Identifiers.Select(x => x.Value).ToList();
            
            var trains = await _trainRepository.GetAllAsync(x => identifiers.Contains(x.ExternalIdentifier.Value), cancellationToken);

            _logger.LogInformation("Retrieved trains. Count: {Count}", trains.Count());

            List<TrainDto> result = [];
            foreach (var train in trains)
            {
                result.Add(new TrainDto(train.Id.Identity,
                                        train.ExternalIdentifier.ToString(),
                                        new TrainParameters(train.Parameters.NumberOfWagons,
                                                            train.Parameters.GrossWeight,
                                                            train.Parameters.NetWeight,
                                                            train.Parameters.Length)));
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get trains");
            
            throw;
        }
    }
}
