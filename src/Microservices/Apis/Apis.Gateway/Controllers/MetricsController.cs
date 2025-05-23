using Asp.Versioning;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metrics.Contracts.Grpc.Impl.Metrics;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("all")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllMetricsAsync([FromQuery] string unifiedNetworkMarkingFrom,
                                                        [FromQuery] string unifiedNetworkMarkingTo,
                                                        [FromQuery] DateTime dateFrom,
                                                        [FromQuery] DateTime dateTo,
                                                        CancellationToken cancellationToken = default)
    {
        var response = await _metricsMicroserviceClient.GetMetricsAsync(new GetMetricsRequest
                                                                        {
                                                                            RailwaySectionFrom = new RailwaySection
                                                                            {
                                                                                UnifiedNetworkMarking = unifiedNetworkMarkingFrom
                                                                            },
                                                                            RailwaySectionTo = new RailwaySection
                                                                            {
                                                                                UnifiedNetworkMarking = unifiedNetworkMarkingTo,
                                                                            },
                                                                            From = Timestamp.FromDateTime(dateFrom),
                                                                            To = Timestamp.FromDateTime(dateTo)
                                                                        },
                                                                        new CallOptions(cancellationToken: cancellationToken));
        
        return Ok(response);
    }
}
