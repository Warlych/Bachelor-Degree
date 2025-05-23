using Abstractions.Domain.Exceptions;

namespace RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

/// <summary>
/// Переход между секциями
/// </summary>
public readonly record struct RailwaySectionTransition
{
    public RailwaySectionId To { get; init; }
    public int Length { get; init; }

    public RailwaySectionTransition(RailwaySectionId to, int length)
    {
        ArgumentNullException.ThrowIfNull(to);
        NegativeOrZeroException.ThrowIfNegative(length);
        
        To = to;
        Length = length;
    }
}
