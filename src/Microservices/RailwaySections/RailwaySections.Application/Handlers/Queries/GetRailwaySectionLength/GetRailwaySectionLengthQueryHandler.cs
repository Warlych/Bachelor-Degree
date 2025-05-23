using System.Collections;
using Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using RailwaySections.Domain.RailwaySections;
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

            return new RailwaySectionLengthResponse(length, railwaySections.Select(x => RailwaySectionDto.Create(x)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get railway section length");
            
            throw;
        }
    }
}
