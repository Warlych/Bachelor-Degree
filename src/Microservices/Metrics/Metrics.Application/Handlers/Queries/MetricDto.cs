using Metrics.Domain.Metrics;

namespace Metrics.Application.Handlers.Queries;

public record MetricDto(Guid Id, RailwaySectionMetrics Metrics, RailwaySection From, RailwaySection To, DateTimeRange TimeRange)
{
    public static MetricDto Create(Metric metric)
    {
        return new MetricDto(metric.Id.Identity,
                             new RailwaySectionMetrics(metric.Metrics.AverageNetWeight,
                                                       metric.Metrics.AverageGrossWeight,
                                                       metric.Metrics.AverageLength,
                                                       metric.Metrics.SectionSpeed,
                                                       metric.Metrics.TechnicalSpeed,
                                                       metric.Metrics.RouteSpeed),
                             new RailwaySection(metric.From.ExternalIdentifier.ToString()),
                             new RailwaySection(metric.To.ExternalIdentifier.ToString()),
                             new DateTimeRange(metric.DateRange.From, metric.DateRange.To));
    }
}

public record RailwaySectionMetrics(double AverageWeightNetto,
                                    double AverageWeightBrutto,
                                    double AverageLength,
                                    double SectionSpeed,
                                    double TechnicalSpeed,
                                    double RouteSpeed);

public record Train(string Number);

public record RailwaySection(string UnifiedNetworkMarking);

public record DateTimeRange(DateTime From, DateTime To); 
