namespace Metrics.Domain.Metrics.ValueObjects.Metrics;

/// <summary>
/// Идентификатор метрики
/// </summary>
/// <param name="Identity"></param>
public readonly record struct MetricId(Guid Identity)
{
    public override string ToString() => Identity.ToString();
}
