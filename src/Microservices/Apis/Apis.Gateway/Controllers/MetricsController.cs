using Apis.Gateway.Models.Metrics.Create;
using Asp.Versioning;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metrics.Contracts.Grpc.Impl.Metrics;
using Microsoft.AspNetCore.Mvc;
using RailwaySection = Metrics.Contracts.Grpc.Impl.Metrics.RailwaySection;

namespace Apis.Gateway.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/{version:apiVersion}/metrics")]
public sealed class MetricsController : ControllerBase
{
    private readonly MetricsMicroservice.MetricsMicroserviceClient _metricsMicroserviceClient;

    public MetricsController(MetricsMicroservice.MetricsMicroserviceClient metricsMicroserviceClient)
    {
        _metricsMicroserviceClient = metricsMicroserviceClient;
    }
    
    [ApiVersion("1.0")]
    [HttpGet("{metricId}")]
    public async Task<IActionResult> GetMetricAsync([FromRoute] Guid metricId, CancellationToken cancellationToken)
    {
        return Ok();
    }
    
    [ApiVersion("1.0")]
    [HttpPost]
    public async Task<IActionResult> CreateMetricAsync(CreateMetricRequest request, CancellationToken cancellationToken)
    {
        var response = await _metricsMicroserviceClient.GetMetricsAsync(new GetMetricsRequest
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
        
        return Ok(response);
    }
}
