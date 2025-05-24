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
}
