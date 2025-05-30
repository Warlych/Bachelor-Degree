using System.Linq.Expressions;
using Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using RailwaySections.Application.Common.Filters;
using RailwaySections.Application.Common.Paginations;
using RailwaySections.Domain.RailwaySections;
using RailwaySections.Domain.RailwaySections.Repositories;

namespace RailwaySections.Application.Handlers.Queries.GetRailwaySections;

public record GetRailwaySectionsQuery(Pagination Pagination, Filter Filter) : IQuery<IEnumerable<RailwaySectionDto>>;

public sealed class GetRailwaySectionsQueryHandler : IQueryHandler<GetRailwaySectionsQuery, IEnumerable<RailwaySectionDto>>
{
    private readonly IRailwaySectionRepository _railwaySectionRepository;
    private readonly ILogger<GetRailwaySectionsQueryHandler> _logger;

    public GetRailwaySectionsQueryHandler(IRailwaySectionRepository railwaySectionRepository, ILogger<GetRailwaySectionsQueryHandler> logger)
    {
        _railwaySectionRepository = railwaySectionRepository;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<RailwaySectionDto>> Handle(GetRailwaySectionsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<RailwaySection, bool>> predicate = null;
            
            if (!String.IsNullOrEmpty(query.Filter.RailwayCode))
            {
                predicate = x => x.Parameters.RailwayCode == query.Filter.RailwayCode;
            }
            
            var sections = await _railwaySectionRepository.GetAsync(predicate, query.Pagination.PageNumber, query.Pagination.PageSize, cancellationToken);
            
            _logger.LogInformation("Retrieved {count} railway sections", sections.Count());

            return sections.Select(x => RailwaySectionDto.Create(x));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get railway sections");
            
            throw;
        }
    }
}
