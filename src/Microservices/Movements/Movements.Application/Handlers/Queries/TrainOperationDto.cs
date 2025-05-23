using Movements.Domain.TrainOperations;

namespace Movements.Application.Handlers.Queries;

public record TrainOperationDto(Guid Id,
                                int Code,
                                Train Train,
                                RailwaySection From,
                                RailwaySection To,
                                DateTime Timestamp)
{
    public static TrainOperationDto Create(TrainOperation operation)
    {
        return new TrainOperationDto(operation.Id.Identity,
                                     (int)operation.Code,
                                     new Train(operation.TrainIdentifier.ToString()),
                                     new RailwaySection(operation.RailwaySectionFromIdentifier.ToString()),
                                     new RailwaySection(operation.RailwaySectionToIdentifier.ToString()),
                                     operation.TimeStamp);
    }
}
    
public record Train(string Number);

public record RailwaySection(string UnifiedNetworkMarking);