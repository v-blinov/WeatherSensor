namespace Weather.SensorService.BL.Models;

public record Event
{
    // TODO : Add fluentValidator
    public DateTime CreatedAt { get; init; }
    public EventData EventData { get; init; } = null!;
}
