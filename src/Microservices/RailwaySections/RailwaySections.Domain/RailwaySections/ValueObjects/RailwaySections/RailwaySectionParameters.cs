namespace RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

/// <summary>
/// Параметры ж/д участка (еср, принадлежность дороге)
/// </summary>
public readonly record struct RailwaySectionParameters
{
    /// <summary>
    /// Код дороги, к которой принадлежит участок
    /// </summary>
    public string RailwayCode { get; init; }
    
    /// <summary>
    /// ЕСР
    /// </summary>
    public string UnifiedNetworkMarking { get; init; }

    public RailwaySectionParameters(string railwayCode, string unifiedNetworkMarking)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(railwayCode);
        ArgumentNullException.ThrowIfNullOrEmpty(unifiedNetworkMarking);
        
        RailwayCode = railwayCode.Trim();
        UnifiedNetworkMarking = unifiedNetworkMarking.Trim();
    }
}
