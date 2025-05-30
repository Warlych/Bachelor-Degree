namespace Apis.Gateway.Models.Metrics.Create;

public record CreateMetricRequest(RailwaySection RailwaySectionFrom, RailwaySection RailwaySectionTo, DateTime From, DateTime To);

public record RailwaySection(string UnifiedNetworkMarking);
