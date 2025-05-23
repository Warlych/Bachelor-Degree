namespace Trains.Domain.Trains.ValueObjects.Trains;

/// <summary>
/// Идентификатор поезда
/// </summary>
/// <param name="Identity"></param>
public readonly record struct TrainId(Guid Identity)
{
    public override string ToString() => Identity.ToString();
}
