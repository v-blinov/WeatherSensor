using Weather.SensorService.BL.Enums;

namespace Weather.SensorService.Models;

public record InitializingSensor
{
    public SensorSettings SensorSettings { get; init; } = null!;
}

public record SensorSettings
{
    public int WorkInterval { get; init; }
    public SensorType SensorType { get; init; }
    public InitializeEventValues InitializeEventValues { get; init; } = null!;
}

public record InitializeEventValues
{
    public double Temperature { get; init; }
    public double AirHumidity { get; init; }
    public double Co2 { get; init; }
}


