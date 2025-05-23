namespace Metrics.Domain.Metrics.ValueObjects.Metrics;

/// <summary>
/// Результаты расчётов параметров поезда: вес, длина, скорости.
/// </summary>
public readonly record struct RailwaySectionMetrics
{
    /// <summary>
    /// Средний вес поезда (нетто), т
    /// </summary>
    public double AverageNetWeight { get; init; }

    /// <summary>
    /// Средний вес поезда (брутто), т
    /// </summary>
    public double AverageGrossWeight { get; init; }

    /// <summary>
    /// Средняя длина поезда, м
    /// </summary>
    public double AverageLength { get; init; }

    /// <summary>
    /// Участковая скорость, км/ч
    /// </summary>
    public double SectionSpeed { get; init; }

    /// <summary>
    /// Техническая скорость, км/ч
    /// </summary>
    public double TechnicalSpeed { get; init; }

    /// <summary>
    /// Маршрутная скорость, км/ч
    /// </summary>
    public double RouteSpeed { get; init; }

    public RailwaySectionMetrics(double averageNetWeight,
                                 double averageGrossWeight,
                                 double averageLength,
                                 double sectionSpeed,
                                 double technicalSpeed,
                                 double routeSpeed)
    {
        AverageNetWeight = averageNetWeight;
        AverageGrossWeight = averageGrossWeight;
        AverageLength = averageLength;
        SectionSpeed = sectionSpeed;
        TechnicalSpeed = technicalSpeed;
        RouteSpeed = routeSpeed;
    }
}