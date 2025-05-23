using Mediator;
using Microsoft.Extensions.Logging;
using RailwaySections.Domain.RailwaySections.Repositories;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

namespace RailwaySections.Application.Handlers.Queries.GetRailwaySection;

public record GetRailwaySectionQuery(RailwaySectionId Id) : IQuery<RailwaySectionDto>;

public sealed class GetRailwaySectionQueryHandler : IQueryHandler<GetRailwaySectionQuery, RailwaySectionDto>
{
    private readonly IRailwaySectionRepository _railwaySectionRepository;
    private readonly ILogger<GetRailwaySectionQueryHandler> _logger;

    public GetRailwaySectionQueryHandler(IRailwaySectionRepository railwaySectionRepository, 
                                         ILogger<GetRailwaySectionQueryHandler> logger)
    {
        _railwaySectionRepository = railwaySectionRepository;
        _logger = logger;
    }

    public async ValueTask<RailwaySectionDto> Handle(GetRailwaySectionQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var railwaySection = await _railwaySectionRepository.GetAsync(x => x.Id == query.Id, cancellationToken);

            ArgumentNullException.ThrowIfNull(railwaySection, nameof(railwaySection));

            _logger.LogInformation($"Retrieved railway section with ID: {query.Id}");

            return RailwaySectionDto.Create(railwaySection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get railway section");
            
            throw;
        }
    }
}
