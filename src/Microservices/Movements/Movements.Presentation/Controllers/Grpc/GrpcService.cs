using System.Net;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mediator;
using Movements.Application.Handlers.Queries.GetTrainOperations;
using Movements.Application.Handlers.Queries.GetTrainsTravellingTimeOnRailwaySection;
using Movements.Contracts.Grpc.Impl.Movements;
using Movements.Domain.TrainOperations.ValueObjects;
using TrainMovementDuration = Movements.Contracts.Grpc.Impl.Movements.TrainMovementDuration;

namespace Movements.Presentation.Controllers.Grpc;

public sealed class GrpcService : MovementsMicroservice.MovementsMicroserviceBase
{
    private readonly ISender _sender;

    public GrpcService(ISender sender)
    {
        _sender = sender;
    }

    public async override Task<GetTrainOperationsResponse> GetTrainOperations(GetTrainOperationsRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new GetTrainOperationsQuery(request.Sections
                                                                               .Select(x => new ExternalIdentifier(x.UnifiedNetworkMarking))),
                                            context.CancellationToken);

            return new GetTrainOperationsResponse
            {
                Success = true,
                TrainOperations = new TrainOperations
                {
                    TrainOperations_ =
                    {
                        result.Select(x => new TrainOperation
                        {
                            Id = x.Id.ToString(),
                            Code = x.Code,
                            Train = new Train
                            {
                                Number = x.Train.Number
                            },
                            From = new RailwaySection
                            {
                                UnifiedNetworkMarking = x.From.UnifiedNetworkMarking
                            },
                            To = new RailwaySection
                            {
                                UnifiedNetworkMarking = x.To.UnifiedNetworkMarking
                            },
                            TimeStamp = Timestamp.FromDateTime(x.Timestamp),
                        })
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new GetTrainOperationsResponse
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

    public async override Task<GetTrainsTravellingTimeOnRailwaySectionResponse> GetTrainsTravellingTimeOnRailwaySection(
        GetTrainsTravellingTimeOnRailwaySectionRequest request,
        ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new GetTrainsTravellingTimeOnRailwaySectionQuery(
                                                new ExternalIdentifier(request.RailwaySectionFrom.UnifiedNetworkMarking),
                                                new ExternalIdentifier(request.RailwaySectionTo.UnifiedNetworkMarking),
                                                request.From.ToDateTime(),
                                                request.To.ToDateTime()),
                                            context.CancellationToken);
            
            return new GetTrainsTravellingTimeOnRailwaySectionResponse
            {
                Success = true,
                TrainMovementDurations = new TrainMovementDurations
                {
                    TrainMovementDurations_ =
                    {
                        result.Select(x => new TrainMovementDuration
                        {
                            Train = new Train
                            {
                                Number = x.Train.Number,
                            },
                            From = new RailwaySection
                            {
                                UnifiedNetworkMarking = x.From.UnifiedNetworkMarking
                            },
                            To = new RailwaySection
                            {
                                UnifiedNetworkMarking = x.To.UnifiedNetworkMarking
                            },
                            StartTime = Timestamp.FromDateTime(x.Start),
                            EndTime = Timestamp.FromDateTime(x.End)
                        })
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new GetTrainsTravellingTimeOnRailwaySectionResponse
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
