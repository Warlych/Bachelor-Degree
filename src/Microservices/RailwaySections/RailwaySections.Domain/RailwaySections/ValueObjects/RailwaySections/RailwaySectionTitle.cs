namespace RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

/// <summary>
/// Название ж/д станции
/// </summary>
public readonly record struct RailwaySectionTitle
{
    /// <summary>
    /// Полное название
    /// </summary>
    public string FullName { get; init; }
    
    /// <summary>
    /// Укороченное название
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Мнемокод
    /// </summary>
    public string? Mnemonic { get; init; }

    public RailwaySectionTitle(string fullName, string name, string? mnemonic)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(fullName, "full name of railway section");
        ArgumentNullException.ThrowIfNullOrEmpty(name, "name of railway section");
        
        FullName = fullName;
        Name = name;
        Mnemonic = mnemonic;
    }
}
