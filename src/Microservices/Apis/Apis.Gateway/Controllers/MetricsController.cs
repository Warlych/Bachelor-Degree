using Apis.Gateway.Models.Metrics.Create;
using Asp.Versioning;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metrics.Contracts.Grpc.Impl.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using CreateMetricRequest = Metrics.Contracts.Grpc.Impl.Metrics.CreateMetricRequest;
using RailwaySection = Metrics.Contracts.Grpc.Impl.Metrics.RailwaySection;

namespace Apis.Gateway.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/{version:apiVersion}/metrics")]
public sealed class MetricsController : ControllerBase
{
    private readonly HybridCache _cache;
    private readonly MetricsMicroservice.MetricsMicroserviceClient _metricsMicroserviceClient;

    public MetricsController(HybridCache cache, MetricsMicroservice.MetricsMicroserviceClient metricsMicroserviceClient)
    {
        _cache = cache;
        _metricsMicroserviceClient = metricsMicroserviceClient;
    }

    [ApiVersion("1.0")]
    [HttpGet("{metricId}")]
    public async Task<IActionResult> GetMetricAsync([FromRoute] Guid metricId, CancellationToken cancellationToken)
    {
        var response = await _cache.GetOrCreateAsync(key: $"metrics:{metricId}",
                                                     factory: async x =>
                                                     {
                                                         return await _metricsMicroserviceClient.GetMetricsAsync(new GetMetricsRequest
                                                                                                                 {
                                                                                                                     Id = metricId.ToString()
                                                                                                                 },
                                                                                                                 new CallOptions(cancellationToken: x));
                                                     },
                                                     cancellationToken: cancellationToken);

        if (response.Success)
        {
            return Ok(response);
        }

        return Problem(title: response.Error.Title, statusCode: response.Error.ErrorCode, detail: response.Error.Message);
    }

    [ApiVersion("1.0")]
    [HttpPost]
    public async Task<IActionResult> CreateMetricAsync([FromBody] Models.Metrics.Create.CreateMetricRequest request, CancellationToken cancellationToken)
    {
        var response = await _metricsMicroserviceClient.CreateMetricsAsync(new CreateMetricRequest
        {
            RailwaySectionFrom = new RailwaySection
            {
                UnifiedNetworkMarking = request.RailwaySectionFrom.UnifiedNetworkMarking
            },
            RailwaySectionTo = new RailwaySection
            {
                UnifiedNetworkMarking = request.RailwaySectionTo.UnifiedNetworkMarking
            },
            From = Timestamp.FromDateTime(request.From),
            To = Timestamp.FromDateTime(request.To)
        }, new CallOptions(cancellationToken: cancellationToken));
        
        if (response.Success)
        {
            return Ok(response);
        }

        return Problem(title: response.Error.Title, statusCode: response.Error.ErrorCode, detail: response.Error.Message);
    }
}
