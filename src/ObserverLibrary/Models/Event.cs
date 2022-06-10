namespace ObserverLibrary.Models;

public record Event
{
    /// <summary>
    /// Идентификатор сенсора
    /// </summary>
    public Guid SensorId { get; init; }
    
    /// <summary>
    /// Дата генерации события
    /// </summary>
    public DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Температура воздуха, в ℃
    /// </summary>
    public double Temperature { get; init; }
    
    /// <summary>
    /// Влажность воздуха, в %
    ///     <remarks>
    ///     норма:<br/>
    ///     30-60% - в помещении<br/>
    ///     50-90% - на улице
    ///     </remarks>
    /// </summary>
    public double AirHumidity { get; init; }

    /// <summary>
    /// Содержание углекислого газа, в ppm
    ///     <remarks>
    ///     норма:<br/>
    ///     1000 ppm - в помещении<br/>
    ///     400 ppm - уличный воздух
    ///     </remarks>
    /// </summary>
    public double Co2 { get; init; }
}
