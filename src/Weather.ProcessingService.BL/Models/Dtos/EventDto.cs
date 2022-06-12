namespace Weather.ProcessingService.BL.Models.Dtos;

public record EventDto
{
    /// <summary>
    ///     Время события в UTC
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Температура воздуха
    /// </summary>
    public double Temperature { get; init; }

    /// <summary>
    ///     Влажность воздуха
    /// </summary>
    public double AirHumidity { get; init; }

    /// <summary>
    ///     Содержание углекислого газа
    /// </summary>
    public double Co2 { get; init; }
}
