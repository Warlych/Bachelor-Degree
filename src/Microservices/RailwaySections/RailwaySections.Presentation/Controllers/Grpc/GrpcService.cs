using System.Net;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mediator;
using RailwaySections.Application.Common.Filters;
using RailwaySections.Application.Handlers.Commands.Graph.Build;
using RailwaySections.Application.Handlers.Commands.Graph.Drop;
using RailwaySections.Application.Handlers.Queries.GetRailwaySection;
using RailwaySections.Application.Handlers.Queries.GetRailwaySectionLength;
using RailwaySections.Application.Handlers.Queries.GetRailwaySections;
using RailwaySections.Contracts.Grpc.Impl.RailwaySections;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;
using Pagination = RailwaySections.Application.Common.Paginations.Pagination;
using RailwaySectionParameters = RailwaySections.Contracts.Grpc.Impl.RailwaySections.RailwaySectionParameters;

namespace RailwaySections.Presentation.Controllers.Grpc;

public sealed class GrpcService : RailwaySectionsMicroservice.RailwaySectionsMicroserviceBase
{
    private readonly ISender _sender;

    public GrpcService(ISender sender)
    {
        _sender = sender;
    }

    public async override Task<GetRailwaySectionResponse> GetRailwaySection(GetRailwaySectionRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.Id, out var railwaySectionId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid railway section id or railway section id is empty."));
            }

            var result = await _sender.Send(new GetRailwaySectionQuery(new RailwaySectionId(railwaySectionId)), context.CancellationToken);

            return new GetRailwaySectionResponse
            {
                Success = true,
                RailwaySection = new RailwaySection
                {
                    Id = result.Id.ToString(),
                    Parameters = new RailwaySectionParameters
                    {
                        RailwayCode = result.RailwayCode,
                        UnifiedNetworkMarking = result.UnifiedNetworkMarking
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new GetRailwaySectionResponse
            {
                Success = false,
                Error = new Error
                {
                    Title = ex.GetType().Name,
                    Message = ex.Message,
                    ErrorCode = (int)HttpStatusCode.InternalServerError,
                    ErrorType = ex.GetType().Name,
                    Details = { { "StackTrace", ex.StackTrace } }
                }
            };
        }
    }

    public async override Task<GetRailwaySectionsResponse> GetRailwaySections(GetRailwaySectionsRequest request, ServerCallContext context)
    {
        try
        {
            var pagination = new Pagination
            {
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };

            var result = await _sender.Send(new GetRailwaySectionsQuery(pagination,
                                                                        new Filter
                                                                        {
                                                                            RailwayCode = request.Filters.RailwayCode
                                                                        }),
                                            context.CancellationToken);

            return new GetRailwaySectionsResponse
            {
                Success = true,
                RailwaySections = new Contracts.Grpc.Impl.RailwaySections.RailwaySections
                {
                    RailwaySections_ =
                    {
                        result.Select(x => new RailwaySection
                        {
                            Id = x.Id.ToString(),
                            Parameters = new RailwaySectionParameters
                            {
                                RailwayCode = x.RailwayCode,
                                UnifiedNetworkMarking = x.UnifiedNetworkMarking
                            }
                        })
                    },
                    Pagination = new Contracts.Grpc.Impl.RailwaySections.Pagination
                    {
                        PageNumber = pagination.PageNumber,
                        PageSize = pagination.PageSize
                    },
                    Filters = request.Filters
                }
            };
        }
        catch (Exception ex)
        {
            return new GetRailwaySectionsResponse
            {
                Success = false,
                Error = new Error
                {
                    Title = ex.GetType().Name,
                    Message = ex.Message,
                    ErrorCode = (int)HttpStatusCode.InternalServerError,
                    ErrorType = ex.GetType().Name,
                    Details =
                    {
                        { "StackTrace", ex.StackTrace }
                    }
                }
            };
        }
    }

    public async override Task<GetRailwaySectionLengthResponse> GetRailwaySectionLength(GetRailwaySectionLengthRequest request,
                                                                                        ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new GetRailwaySectionLengthQuery(
                                                new Domain.RailwaySections.ValueObjects.RailwaySections.RailwaySectionParameters(
                                                    request.From.RailwayCode,
                                                    request.From.UnifiedNetworkMarking),
                                                new Domain.RailwaySections.ValueObjects.RailwaySections.RailwaySectionParameters(
                                                    request.To.RailwayCode,
                                                    request.To.UnifiedNetworkMarking)),
                                            context.CancellationToken);

            return new GetRailwaySectionLengthResponse
            {
                Success = true,
                RailwaySectionLength = new RailwaySectionLength
                {
                    Length = (uint)result.Length,
                    PercentageMainSections = (double)result.PercentageMainSections,
                    PercentageTechinalStations = (double)result.PercentageTechinalStations,
                    PercentageAuxiliarySections = (double)result.PercentageAuxiliarySections,
                    RailwaySections =
                    {
                        result.Sections.Select(x => new RailwaySection
                        {
                            Id = x.Id.ToString(),
                            Parameters = new RailwaySectionParameters
                            {
                                RailwayCode = x.RailwayCode,
                                UnifiedNetworkMarking = x.UnifiedNetworkMarking
                            }
                        })
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new GetRailwaySectionLengthResponse
            {
                Success = false,
                Error = new Error
                {
                    Title = ex.GetType().Name,
                    Message = ex.Message,
                    ErrorCode = (int)HttpStatusCode.InternalServerError,
                    ErrorType = ex.GetType().Name,
                    Details = { { "StackTrace", ex.StackTrace } }
                }
            };
        }
    }

    public async override Task<BuildGraphResponse> BuildGraph(Empty request, ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new BuildGraphCommand(), context.CancellationToken);

            return new BuildGraphResponse
            {
                Success = result
            };
        }
        catch (Exception ex)
        {
            return new BuildGraphResponse
            {
                Success = false,
                Error = new Error
                {
                    Title = ex.GetType().Name,
                    Message = ex.Message,
                    ErrorCode = (int)HttpStatusCode.InternalServerError,
                    ErrorType = ex.GetType().Name,
                    Details = { { "StackTrace", ex.StackTrace } }
                }
            };
        }
    }

    public async override Task<DropGraphReponse> DropGraph(Empty request, ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new DropGraphCommand(), context.CancellationToken);

            return new DropGraphReponse
            {
                Success = result
            };
        }
        catch (Exception ex)
        {
            return new DropGraphReponse
            {
                Success = false,
                Error = new Error
                {
                    Title = ex.GetType().Name,
                    Message = ex.Message,
                    ErrorCode = (int)HttpStatusCode.InternalServerError,
                    ErrorType = ex.GetType().Name,
                    Details = { { "StackTrace", ex.StackTrace } }
                }
            };
        }
    }
}