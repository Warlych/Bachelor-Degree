using System.Net;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mediator;
using Metrics.Application.Handlers.Commands.Create;
using Metrics.Application.Handlers.Queries.GetMetric;
using Metrics.Contracts.Grpc.Impl.Metrics;
using Metrics.Domain.Metrics.ValueObjects;
using Metrics.Domain.Metrics.ValueObjects.Metrics;
using DateTimeRange = Metrics.Contracts.Grpc.Impl.Metrics.DateTimeRange;
using RailwaySectionMetrics = Metrics.Contracts.Grpc.Impl.Metrics.RailwaySectionMetrics;

namespace Metrics.Presentation.Controllers.Grpc;

public sealed class GrpcService : MetricsMicroservice.MetricsMicroserviceBase
{
    private readonly ISender _sender;

    public GrpcService(ISender sender)
    {
        _sender = sender;
    }

    public async override Task<GetMetricsReponse> GetMetrics(GetMetricsRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.Id, out var id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid metric id or metric id is empty."));
            }
            
            var result = await _sender.Send(new GetMetricQuery(new MetricId(id)), context.CancellationToken);

            return new GetMetricsReponse
            {
                Success = true,
                Metric = new Metric
                {
                    Id = result.Id.ToString(),
                    Metrics = new RailwaySectionMetrics
                    {
                        AverageWeightNetto = result.Metrics.AverageWeightNetto,
                        AverageWeightBrutto = result.Metrics.AverageWeightBrutto,
                        AverageLength = result.Metrics.AverageLength,
                        SectionSpeed = result.Metrics.SectionSpeed,
                        TechnicalSpeed = result.Metrics.TechnicalSpeed,
                        RouteSpeed = result.Metrics.RouteSpeed
                    },
                    From = new RailwaySection
                    {
                        UnifiedNetworkMarking = result.From.UnifiedNetworkMarking
                    },
                    To = new RailwaySection
                    {
                        UnifiedNetworkMarking = result.To.UnifiedNetworkMarking
                    },
                    TimeRange = new DateTimeRange
                    {
                        From = Timestamp.FromDateTime(result.TimeRange.From.ToUniversalTime()),
                        To = Timestamp.FromDateTime(result.TimeRange.To.ToUniversalTime())
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new GetMetricsReponse
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

    public async override Task<CreateMetricResponse> CreateMetrics(CreateMetricRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new CreateMetricCommand(new ExternalIdentifier(request.RailwaySectionFrom.UnifiedNetworkMarking),
                                                                    new ExternalIdentifier(request.RailwaySectionTo.UnifiedNetworkMarking),
                                                                    request.From.ToDateTime(),
                                                                    request.To.ToDateTime()));

            return new CreateMetricResponse
            {
                Success = true,
                Metric = new Metric
                {
                    Id = result.Id.ToString(),
                    Metrics = new RailwaySectionMetrics
                    {
                        AverageWeightNetto = result.Metrics.AverageWeightNetto,
                        AverageWeightBrutto = result.Metrics.AverageWeightBrutto,
                        AverageLength = result.Metrics.AverageLength,
                        SectionSpeed = result.Metrics.SectionSpeed,
                        TechnicalSpeed = result.Metrics.TechnicalSpeed,
                        RouteSpeed = result.Metrics.RouteSpeed
                    },
                    From = new RailwaySection
                    {
                        UnifiedNetworkMarking = result.From.UnifiedNetworkMarking
                    },
                    To = new RailwaySection
                    {
                        UnifiedNetworkMarking = result.To.UnifiedNetworkMarking
                    },
                    TimeRange = new DateTimeRange
                    {
                        From = Timestamp.FromDateTime(result.TimeRange.From.ToUniversalTime()),
                        To = Timestamp.FromDateTime(result.TimeRange.To.ToUniversalTime())
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new CreateMetricResponse
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
