namespace Movements.Domain.TrainOperations.ValueObjects.TrainOperations;

/// <summary>
/// Идентфикатор операции над поездом на ж/д пути
/// </summary>
/// <param name="Identity"></param>
public readonly record struct TrainOperationId(Guid Identity)
{
    public override string ToString() => Identity.ToString();
}
