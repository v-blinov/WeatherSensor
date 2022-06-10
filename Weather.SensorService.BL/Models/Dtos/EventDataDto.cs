namespace Weather.SensorService.BL.Models.Dtos;

public record EventDataDto
{
    public double Temperature { get; init; }
    public double AirHumidity { get; init; }
    public double Co2 { get; init; }
}
