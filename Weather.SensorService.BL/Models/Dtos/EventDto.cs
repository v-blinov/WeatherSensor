namespace Weather.SensorService.BL.Models.Dtos;

public record EventDto
{
    public DateTime CreatedAt { get; init; }
    public EventDataDto EventData { get; init; } = null!;
}
