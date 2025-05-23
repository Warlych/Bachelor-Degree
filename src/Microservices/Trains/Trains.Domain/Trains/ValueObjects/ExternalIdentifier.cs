namespace Trains.Domain.Trains.ValueObjects;

public readonly record struct ExternalIdentifier
{
    /// <summary>
    /// Идентификатор сущности в других системах
    /// </summary>
    public string Value { get; init; }
    
    public ExternalIdentifier(string value)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(value);
        Value = value.Trim();
    }
    
    public override string ToString() => Value.ToString();
}
