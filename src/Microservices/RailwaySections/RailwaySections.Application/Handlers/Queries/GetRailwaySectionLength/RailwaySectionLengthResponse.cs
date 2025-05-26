using RailwaySections.Domain.RailwaySections;

namespace RailwaySections.Application.Handlers.Queries.GetRailwaySectionLength;

public record RailwaySectionLengthResponse(int Length,
                                           IEnumerable<RailwaySectionDto> Sections,
                                           double PercentageMainSections,
                                           double PercentageTechinalStations,
                                           double PercentageAuxiliarySections);
