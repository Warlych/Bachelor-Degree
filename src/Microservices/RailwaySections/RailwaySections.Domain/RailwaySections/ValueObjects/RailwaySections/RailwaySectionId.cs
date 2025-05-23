namespace RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

/// <summary>
/// Идентификатор ж/д секции
/// </summary>
/// <param name="Identity"></param>
public readonly record struct RailwaySectionId(Guid Identity)
{
    public override string ToString() => Identity.ToString();
}
