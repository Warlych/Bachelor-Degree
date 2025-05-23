namespace Movements.Domain.TrainOperations.Enums;

/// <summary>
/// Операции, связанные с движением и маневрами поездов
/// </summary>
public enum TrainOperationCodes
{
    /// <summary>
    /// Не определена
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Прибытие
    /// </summary>
    Arrival = 1,
    
    /// <summary>
    /// Готовность
    /// </summary>
    Ready = 2,

    /// <summary>
    /// Отправление
    /// </summary>
    Departure = 3,

    /// <summary>
    /// Сходу
    /// </summary>
    PassThrough = 4,

    /// <summary>
    /// Расформирование
    /// </summary>
    Disbanding = 5,

    /// <summary>
    /// Задержка в продвижении
    /// </summary>
    Delay = 6,

    /// <summary>
    /// Сформирование
    /// </summary>
    Forming = 7,

    /// <summary>
    /// Изменение индекса поезда
    /// </summary>
    TrainIndexChange = 8,

    /// <summary>
    /// Перестановка
    /// </summary>
    Rearrangement = 10,

    /// <summary>
    /// Заход на путь
    /// </summary>
    EnteringTrack = 11,

    /// <summary>
    /// Образован соединенный
    /// </summary>
    ConnectedFormed = 12,

    /// <summary>
    /// Включение в соединенный
    /// </summary>
    ConnectedIncluded = 13,

    /// <summary>
    /// Прицепка
    /// </summary>
    Coupling = 15,

    /// <summary>
    /// Отцепка
    /// </summary>
    Uncoupling = 16,

    /// <summary>
    /// Разъединение соединенного
    /// </summary>
    ConnectedSplit = 18,

    /// <summary>
    /// Исключение из соединенного
    /// </summary>
    ConnectedExcluded = 19,

    /// <summary>
    /// Прибытие в границы станции
    /// </summary>
    ArrivalToStationArea = 21,

    /// <summary>
    /// Отправление за границы станции
    /// </summary>
    DepartureFromStationArea = 23,

    /// <summary>
    /// Образован объединенный
    /// </summary>
    UnifiedFormed = 32,

    /// <summary>
    /// Включение в объединенный
    /// </summary>
    UnifiedIncluded = 33,

    /// <summary>
    /// Разъединение объединенного
    /// </summary>
    UnifiedSplit = 38,

    /// <summary>
    /// Исключение из объединенного
    /// </summary>
    UnifiedExcluded = 39,

    /// <summary>
    /// Окончание движения
    /// </summary>
    EndOfMovement = 40,

    /// <summary>
    /// Начало движения
    /// </summary>
    StartOfMovement = 41,

    /// <summary>
    /// Корректировка ТГНЛ
    /// </summary>
    TGNLAdjustment = 36,

    /// <summary>
    /// ТГНЛ
    /// </summary>
    TGNL = 37,

    /// <summary>
    /// Смена номера
    /// </summary>
    TrainNumberChange = 68,

    /// <summary>
    /// Прицепка локомотива
    /// </summary>
    LocomotiveCoupling = 90,

    /// <summary>
    /// Сдача по стыку
    /// </summary>
    HandoverAtJoint = 71,

    /// <summary>
    /// Прием по стыку
    /// </summary>
    AcceptanceAtJoint = 72
}