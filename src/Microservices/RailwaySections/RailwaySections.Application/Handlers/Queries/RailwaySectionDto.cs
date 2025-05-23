using RailwaySections.Domain.RailwaySections;

namespace RailwaySections.Application.Handlers.Queries;

public record RailwaySectionDto(Guid Id,
                                string FullName,
                                string Name,
                                string? Mnemonic,
                                string RailwayCode,
                                string UnifiedNetworkMarking)
{
    public static RailwaySectionDto Create(RailwaySection railwaySection)
    {
        return new RailwaySectionDto(railwaySection.Id.Identity,
                                     railwaySection.Title.FullName,
                                     railwaySection.Title.Name,
                                     railwaySection.Title.Mnemonic,
                                     railwaySection.Parameters.RailwayCode,
                                     railwaySection.Parameters.UnifiedNetworkMarking);
    } 
}
