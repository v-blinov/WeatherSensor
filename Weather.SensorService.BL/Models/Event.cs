namespace Weather.SensorService.BL.Models;

public record Event
{
    public DateTime CreatedAt { get; init; }
    public EventData EventData { get; init; } = null!;
}
