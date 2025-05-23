namespace Movements.Contracts.Contracts;

/// <summary>
/// Событие представляющее операцию над поездов на ж/д участке
/// </summary>
public sealed class TrainOperationCreatedEvent
{
    /// <summary>
    /// Идентификатор операции
    /// </summary> 
    public Guid Id { get; set; }
    
    /// <summary>
    /// Код операции
    /// </summary>
    public int Code { get; set; }
    
    /// <summary>
    /// Поезд участвующий в операции
    /// </summary>
    public Train Train { get; set; }
    
    /// <summary>
    /// Станция начала движения
    /// </summary>
    public RailwaySection From { get; set; }
    
    /// <summary>
    /// Станция окончания движения
    /// </summary>
    public RailwaySection To { get; set; }
    
    /// <summary>
    /// Метка времени операции
    /// </summary>
    public DateTime TimeStamp { get; set; }
}

/// <summary>
/// Представление поезда в событии
/// </summary>
public sealed class Train
{
    /// <summary>
    /// Номер поезда
    /// </summary>
    public string Number { get; set; }
}

/// <summary>
/// Представление станции в событии
/// </summary>
public sealed class RailwaySection
{
    /// <summary>
    /// ЕСР станции
    /// </summary>
    public string UnifiedNetworkMarking { get; set; } 
}