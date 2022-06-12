namespace Weather.SensorService.BL.Models.Dtos;

public record SensorDto
{
    public Guid Id { get; init; }
    public SensorSettingDto SensorSettings { get; init; } = null!;
    public EventDto Event { get; init; } = null!;
}
