using Weather.SensorService.BL.Enums;

namespace Weather.SensorService.BL.Models;

public record SensorSettings
{
    // TODO : Add fluentValidator
    public int WorkInterval { get; init; } 
    public SensorType Type { get; init; } 
}