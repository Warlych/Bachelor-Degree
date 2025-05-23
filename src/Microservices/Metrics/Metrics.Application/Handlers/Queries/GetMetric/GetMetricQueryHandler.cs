using Mediator;
using Metrics.Domain.Metrics.Entities;
using Metrics.Domain.Metrics.Repositories;
using Metrics.Domain.Metrics.ValueObjects.Metrics;
using Microsoft.Extensions.Logging;

namespace Metrics.Application.Handlers.Queries.GetMetric;

public record GetMetricQuery(MetricId MetricId) : IQuery<MetricDto>;

public sealed class GetMetricQueryHandler : IQueryHandler<GetMetricQuery, MetricDto>
{
    private readonly IMetricsRepository _metricsRepository;
    private readonly ILogger<GetMetricQueryHandler> _logger;

    public GetMetricQueryHandler(IMetricsRepository metricsRepository, ILogger<GetMetricQueryHandler> logger)
    {
        _metricsRepository = metricsRepository;
        _logger = logger;
    }

    public async ValueTask<MetricDto> Handle(GetMetricQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var metric = await _metricsRepository.GetAsync(x => x.Id == query.MetricId, cancellationToken);
            
            _logger.LogInformation($"Retrieved train with ID: {metric.Id}");
            
            return MetricDto.Create(metric);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get metric");
            
            throw;
        }
    }
}
