using System.Collections.ObjectModel;
using Abstractions.Domain.AggregateRoot;
using Metrics.Domain.Metrics.Entities;
using Metrics.Domain.Metrics.ValueObjects;
using Metrics.Domain.Metrics.ValueObjects.Metrics;
using Metrics.Domain.Metrics.ValueObjects.RailwaySections;
using Metrics.Domain.Metrics.ValueObjects.Trains;

namespace Metrics.Domain.Metrics;

/// <summary>
/// Объект метрики (по скорости, длине, весу, на ж/д участке)
/// </summary>
public sealed class Metric : AggregateRoot<MetricId>
{
    private List<Train> _trains = [];

    public RailwaySection From { get; private set; }
    public RailwaySection To { get; private set; }

    public DateTimeRange DateRange { get; private set; }

    public RailwaySectionMetrics Metrics { get; private set; }

    public ReadOnlyCollection<Train> Trains => _trains.AsReadOnly();

    /// <summary>
    /// Только для EF core
    /// </summary>
    private Metric() : base(default)
    {
    }

    public Metric(MetricId id,
                  RailwaySection from,
                  RailwaySection to,
                  DateTimeRange dateRange,
                  RailwaySectionMetrics metrics,
                  IEnumerable<Train> trains) 
        : base(id)
    {
        From = from;
        To = to;
        DateRange = dateRange;
        Metrics = metrics;
        _trains.AddRange(trains);
    }

    public static Metric Create(RailwaySection from,
                                RailwaySection to,
                                DateTimeRange dateRange,
                                RailwaySectionMetrics metrics,
                                IEnumerable<Train> trains)
    {
        return new Metric(new MetricId(Guid.CreateVersion7()),
                          from,
                          to,
                          dateRange,
                          metrics,
                          trains);
    }

    public static RailwaySection CreateSection(ExternalIdentifier externalId)
    {
        return new RailwaySection(new RailwaySectionId(Guid.CreateVersion7()), externalId);
    }

    public static Train CreateTrain(ExternalIdentifier externalId)
    {
        return new Train(new TrainId(Guid.CreateVersion7()), externalId);
    }
}
