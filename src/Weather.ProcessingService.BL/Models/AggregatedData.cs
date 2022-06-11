namespace Weather.ProcessingService.BL.Models;

public record AggregatedData
{
    public Guid SensorId { get; init; }
    public Period? Period { get; init; }

    public double AverageTemperature { get; init; }
    public double AverageAirHumidity { get; init; }
    public double MaxCo2 { get; init; }
    public double MinCo2 { get; init; }
}
