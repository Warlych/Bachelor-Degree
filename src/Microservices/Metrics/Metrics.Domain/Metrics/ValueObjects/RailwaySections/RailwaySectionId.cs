namespace Metrics.Domain.Metrics.ValueObjects.RailwaySections;

public readonly record struct RailwaySectionId(Guid Identity)
{
    public override string ToString() => Identity.ToString();
}
