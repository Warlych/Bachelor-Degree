using Apis.Gateway.Models.Filters;
using Asp.Versioning;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using RailwaySections.Contracts.Grpc.Impl.RailwaySections;
using Pagination = Apis.Gateway.Models.Paginations.Pagination;

namespace Apis.Gateway.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/{version:apiVersion}/railwaysections")]
public sealed class RailwaySectionsController : ControllerBase
{
    private readonly RailwaySectionsMicroservice.RailwaySectionsMicroserviceClient _railwaySectionsMicroserviceClient;

    public RailwaySectionsController(RailwaySectionsMicroservice.RailwaySectionsMicroserviceClient railwaySectionsMicroserviceClient)
    {
        _railwaySectionsMicroserviceClient = railwaySectionsMicroserviceClient;
    }
    
    [ApiVersion("1.0")]
    [HttpPost("graph/build")]
    public async Task<IActionResult> BuildGraphAsync(CancellationToken cancellationToken)
    {
        var response = await _railwaySectionsMicroserviceClient.BuildGraphAsync(new Empty(), new CallOptions(cancellationToken: cancellationToken));
        
        return Ok(response);
    }

    [ApiVersion("1.0")]
    [HttpPost("graph/drop")]
    public async Task<IActionResult> DropGraphAsync(CancellationToken cancellationToken)
    {
        var response = await _railwaySectionsMicroserviceClient.DropGraphAsync(new Empty(), new CallOptions(cancellationToken: cancellationToken));
        
        return Ok(response);
    }

    [ApiVersion("1.0")]
    [HttpGet]
    public async Task<IActionResult> GetRailwaySectionsAsync([FromQuery] Pagination pagination, [FromQuery] RailwaySectionFilters filters, CancellationToken cancellationToken)
    {
        var response = await _railwaySectionsMicroserviceClient.GetRailwaySectionsAsync(new GetRailwaySectionsRequest
        {
            Pagination = new RailwaySections.Contracts.Grpc.Impl.RailwaySections.Pagination
            {
                PageSize = pagination.PageSize,
                PageNumber = pagination.PageNumber
            },
            Filters = new Filters
            {
                RailwayCode = filters.RailwayCode
            }
        }, new CallOptions(cancellationToken: cancellationToken));
        
        return Ok(response);
    }
}
