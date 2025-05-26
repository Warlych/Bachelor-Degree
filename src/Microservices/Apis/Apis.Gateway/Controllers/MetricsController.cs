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

    [HttpPost("graph/build")]
    public async Task<IActionResult> BuildGraphAsync(CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpPost("graph/drop")]
    public async Task<IActionResult> DropGraphAsync(CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpGet("{metricId}")]
    public async Task<IActionResult> GetMetricAsync([FromRoute] Guid metricId, CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMetricAsync(CancellationToken cancellationToken)
    {
        return Ok();
    }
}
