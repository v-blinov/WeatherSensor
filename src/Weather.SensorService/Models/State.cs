namespace Weather.SensorService.Models;

public record State
{
    public double Temperature { get; init; }
    public double AirHumidity { get; init; }
    public double Co2 { get; init; }
}
