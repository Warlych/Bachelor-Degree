namespace Metrics.Domain.Metrics.ValueObjects.Trains;

public readonly record struct TrainId(Guid Identity)
{
    public override string ToString() => Identity.ToString();
}
