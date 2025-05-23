namespace Trains.Application.Handlers.Queries;

public sealed record TrainDto(Guid Id, string ExternalIdentifier, TrainParameters Parameters);

public sealed record TrainParameters(int NumberOfWagons, double GrossWeight, double NetWeight, double Lenght);
