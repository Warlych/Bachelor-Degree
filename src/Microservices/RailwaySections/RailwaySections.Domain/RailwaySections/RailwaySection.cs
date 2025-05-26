using Abstractions.Domain.AggregateRoot;
using RailwaySections.Domain.RailwaySections.Enums;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

namespace RailwaySections.Domain.RailwaySections;

/// <summary>
/// Объект ж/д участка
/// </summary>
public sealed class RailwaySection : AggregateRoot<RailwaySectionId>
{
    private List<RailwaySectionTransition> _transitions = [];

    /// <summary>
    /// Тип ж/д участка
    /// </summary>
    public RailwaySectionTypes Type { get; private set; }
    
    /// <summary>
    /// Название ж/д участка
    /// </summary>
    public RailwaySectionTitle Title { get; private set; }

    /// <summary>
    /// Параметры ж/д участка (еср, код дороги)
    /// </summary>
    public RailwaySectionParameters Parameters { get; private set; }

    /// <summary>
    /// Переходы между секциями
    /// </summary>
    public IReadOnlyCollection<RailwaySectionTransition> Transitions => _transitions.AsReadOnly();

    
    public RailwaySection(RailwaySectionId id,
                          RailwaySectionTypes type,
                          RailwaySectionTitle title,
                          RailwaySectionParameters parameters,
                          IEnumerable<RailwaySectionTransition> transitions)
        : base(id)
    {
        Type = type;
        Title = title;
        Parameters = parameters;
        _transitions.AddRange(transitions);
    }
}
