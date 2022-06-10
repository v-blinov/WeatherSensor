using Weather.SensorService.BL.Enums;

namespace Weather.SensorService.BL.Models;

public record SensorSettings
{
    public int WorkInterval { get; init; } 
    public SensorType Type { get; init; } 
}