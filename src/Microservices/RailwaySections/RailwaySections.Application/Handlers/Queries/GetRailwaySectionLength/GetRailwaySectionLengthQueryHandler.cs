using System.Collections;
using Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using RailwaySections.Domain.RailwaySections;
using RailwaySections.Domain.RailwaySections.Enums;
using RailwaySections.Domain.RailwaySections.Repositories;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

namespace RailwaySections.Application.Handlers.Queries.GetRailwaySectionLength;

public record GetRailwaySectionLengthQuery(RailwaySectionParameters From, RailwaySectionParameters To)
    : IQuery<RailwaySectionLengthResponse>;

public sealed class GetRailwaySectionLengthQueryHandler : IQueryHandler<GetRailwaySectionLengthQuery, RailwaySectionLengthResponse>
{
    private readonly IRailwaySectionRepository _railwaySectionRepository;
    private readonly ILogger<GetRailwaySectionLengthQueryHandler> _logger;

    public GetRailwaySectionLengthQueryHandler(IRailwaySectionRepository railwaySectionRepository,
                                               ILogger<GetRailwaySectionLengthQueryHandler> logger)
    {
        _railwaySectionRepository = railwaySectionRepository;
        _logger = logger;
    }

    public async ValueTask<RailwaySectionLengthResponse> Handle(GetRailwaySectionLengthQuery query,
                                                                CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _logger.BeginScope("Railway section length query");
            
            var from = await _railwaySectionRepository.GetAsync(x => x.Parameters.UnifiedNetworkMarking == query.From.UnifiedNetworkMarking, cancellationToken);

            ArgumentNullException.ThrowIfNull(from, nameof(from));

            _logger.LogInformation("Retrieved railway section with Railway Code: {code}", from.Parameters.UnifiedNetworkMarking);
            
            var to = await _railwaySectionRepository.GetAsync(x => x.Parameters.UnifiedNetworkMarking == query.To.UnifiedNetworkMarking, cancellationToken);

            ArgumentNullException.ThrowIfNull(to, nameof(to));

            _logger.LogInformation("Retrieved railway section with Railway Code: {code}", to.Parameters.UnifiedNetworkMarking);
            
            var (length, railwaySections) = await _railwaySectionRepository.GetRailwaySectionLengthAsync(from, to, cancellationToken);
            
            _logger.LogInformation("Retrieved railway section length: {length} in between {@from} and {@to}", length, from, to);

            var totalSections = railwaySections.Count();
            var totalMainSections = 0;
            var totalTechnicalSections = 0;
            var totalAuxiliarySections = 0;
            
            foreach (var railwaySection in railwaySections)
            {
                if (railwaySection.Type is RailwaySectionTypes.Sectional)
                {
                    totalMainSections++;

                    continue;
                }

                if (railwaySection.Type is RailwaySectionTypes.Intermediate)
                {
                    totalAuxiliarySections++;
                    
                    continue;
                }
                
                totalTechnicalSections++;
            }

            return new RailwaySectionLengthResponse(length,
                                                    railwaySections.Select(x => RailwaySectionDto.Create(x)),
                                                    CalculatePercentage(totalSections, totalMainSections),
                                                    CalculatePercentage(totalSections, totalTechnicalSections),
                                                    CalculatePercentage(totalSections, totalAuxiliarySections));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get railway section length");
            
            throw;
        }
    }

    private double CalculatePercentage(double total, double part)
    {
        if (total is default(double))
        {
            return default;
        }

        return part / total * 100d;
    }
}
