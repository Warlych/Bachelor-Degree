using System.Net;
using Grpc.Core;
using Mediator;
using Trains.Application.Handlers.Queries.GetTrain;
using Trains.Application.Handlers.Queries.GetTrains;
using Trains.Contracts.Grpc.Impl.Trains;
using Trains.Domain.Trains.ValueObjects;
using TrainParameters = Trains.Contracts.Grpc.Impl.Trains.TrainParameters;

namespace Trains.Presentation.Controllers.Grpc;

public sealed class GrpcService : TrainsMicroservice.TrainsMicroserviceBase
{
    private readonly ISender _sender;

    public GrpcService(ISender sender)
    {
        _sender = sender;
    }

    public async override Task<GetTrainResponse> GetTrain(GetTrainRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new GetTrainQuery(new ExternalIdentifier(request.ExternalIdentifier)), context.CancellationToken);

            return new GetTrainResponse
            {
                Succes = true,
                Train = new Train
                {
                    Id = result.Id.ToString(),
                    ExternalIdentifier = result.ExternalIdentifier,
                    Parameters = new TrainParameters
                    {
                        NumberOfWagons = result.Parameters.NumberOfWagons,
                        GrossWeight = result.Parameters.GrossWeight,
                        NetWeight = result.Parameters.NetWeight,
                        Length = result.Parameters.Lenght
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new GetTrainResponse
            {
                Succes = false,
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

    public async override Task<GetTrainsResponse> GetTrains(GetTrainsRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _sender.Send(new GetTrainsQuery(request.ExternalIdentifier.Select(x => new ExternalIdentifier(x))),
                                            context.CancellationToken);

            return new GetTrainsResponse
            {
                Succes = true,
                Trains = new Contracts.Grpc.Impl.Trains.Trains
                {
                    Trains_ =
                    {
                        result.Select(x => new Train
                        {
                            Id = x.Id.ToString(),
                            ExternalIdentifier = x.ExternalIdentifier,
                            Parameters = new TrainParameters
                            {
                                NumberOfWagons = x.Parameters.NumberOfWagons,
                                GrossWeight = x.Parameters.GrossWeight,
                                NetWeight = x.Parameters.NetWeight,
                                Length = x.Parameters.Lenght
                            }
                        })
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new GetTrainsResponse
            {
                Succes = false,
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
