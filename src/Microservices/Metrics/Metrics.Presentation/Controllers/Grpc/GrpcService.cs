using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mediator;
using Metrics.Application.Handlers.Commands.Create;
using Metrics.Contracts.Grpc.Impl.Metrics;
using Metrics.Domain.Metrics.ValueObjects;

namespace Metrics.Presentation.Controllers.Grpc;

public sealed class GrpcService : MetricsMicroservice.MetricsMicroserviceBase
{
    private readonly ISender _sender;

    public GrpcService(ISender sender)
    {
        _sender = sender;
    }

    public async override Task<GetMetricsResponse> GetMetrics(GetMetricsRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new CreateMetricCommand(new ExternalIdentifier(request.RailwaySectionFrom.UnifiedNetworkMarking),
                                                                    new ExternalIdentifier(request.RailwaySectionTo.UnifiedNetworkMarking),
                                                                    request.From.ToDateTime(),
                                                                    request.To.ToDateTime()));

            return new GetMetricsResponse
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
            return new GetMetricsResponse
            {
                Success = false,
                Error = new Error
                {
                    Message = ex.Message,
                    AdditionalDetails = ex.StackTrace
                }
            };
        }
    }
}
