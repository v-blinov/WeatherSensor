namespace Weather.SensorService.BL.Models;

public record EventData
{
    public double Temperature { get; init; }
    public double AirHumidity { get; init; }
    public double Co2 { get; init; }
}