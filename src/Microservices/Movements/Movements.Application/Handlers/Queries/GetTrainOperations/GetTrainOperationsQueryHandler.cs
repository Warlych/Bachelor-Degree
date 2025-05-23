using Mediator;
using Microsoft.Extensions.Logging;
using Movements.Domain.TrainOperations.Repositories;
using Movements.Domain.TrainOperations.ValueObjects;

namespace Movements.Application.Handlers.Queries.GetTrainOperations;

public record GetTrainOperationsQuery(IEnumerable<ExternalIdentifier> RailwaySections) : IQuery<IEnumerable<TrainOperationDto>>;

public sealed class GetTrainOperationsQueryHandler : IQueryHandler<GetTrainOperationsQuery, IEnumerable<TrainOperationDto>>
{
    private readonly ITrainOperationRepository _trainOperationRepository;
    private readonly ILogger<GetTrainOperationsQueryHandler> _logger;

    public GetTrainOperationsQueryHandler(ITrainOperationRepository trainOperationRepository, ILogger<GetTrainOperationsQueryHandler> logger)
    {
        _trainOperationRepository = trainOperationRepository;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<TrainOperationDto>> Handle(GetTrainOperationsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            List<TrainOperationDto> result = [];

            await foreach (var operation in _trainOperationRepository
                               .GetAllAsAsyncEnumerableAsync(x => query.RailwaySections
                                                      .Any(y => y == x.RailwaySectionFromIdentifier || y == x.RailwaySectionToIdentifier),
                                            cancellationToken))
            {
                result.Add(TrainOperationDto.Create(operation));
            }
            
            _logger.LogInformation("Retrieved train operations. Count {count}", result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get train operations");

            throw;
        }
    }
}
