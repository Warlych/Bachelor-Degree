namespace RailwaySections.Domain.RailwaySections.Enums;

public enum RailwaySectionTypes
{
    /// <summary>
    /// Станция, предназначенная для расформирования и формирования поездов
    /// </summary>
    Sorting,

    /// <summary>
    /// Станция на перегоне между участковыми или другими крупными станциями
    /// </summary>
    Intermediate,

    /// <summary>
    /// Основная станция на участке пути, где может производиться смена локомотивов или экипажа
    /// </summary>
    Sectional,

    /// <summary>
    /// Станция, обслуживающая грузовые перевозки
    /// </summary>
    Freight,

    /// <summary>
    /// Станция, обслуживающая пассажирские перевозки
    /// </summary>
    Passenger
}
