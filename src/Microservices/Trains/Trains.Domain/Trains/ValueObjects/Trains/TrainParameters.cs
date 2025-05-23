using Abstractions.Domain.Exceptions;

namespace Trains.Domain.Trains.ValueObjects.Trains;

/// <summary>
/// Статистические параметры поезда
/// </summary>
public readonly record struct TrainParameters
{
    /// <summary>
    /// Количество вагонов
    /// </summary>
    public int NumberOfWagons { get; init; }
    
    /// <summary>
    /// Вес в брутто
    /// </summary>
    public double GrossWeight { get; init; }

    /// <summary>
    /// Вес в нетто
    /// </summary>
    public double NetWeight { get; init; }

    /// <summary>
    /// Длина поезда в условных вагонах
    /// </summary>
    public double Length { get; init; }

    public TrainParameters(int numberOfWagons, double grossWeight, double netWeight, double length)
    {
        //NegativeOrZeroException.ThrowIfNegativeOrZero(numberOfWagons, "number of wagons");
        //NegativeOrZeroException.ThrowIfNegativeOrZero(grossWeight, "gross train weight");
        //NegativeOrZeroException.ThrowIfNegativeOrZero(netWeight, "net train weight");
        //NegativeOrZeroException.ThrowIfNegativeOrZero(length, "length");

        NumberOfWagons = numberOfWagons;
        GrossWeight = grossWeight;
        NetWeight = netWeight;
        Length = length;
    }
}
