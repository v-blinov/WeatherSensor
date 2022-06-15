using Weather.SensorService.BL.Enums;

namespace Weather.SensorService.BL.Models;

public record SensorSettings
{
    private int workInterval;
    public int WorkInterval
    {
        get
        {
            return workInterval;
        }
        init
        {
            workInterval = workInterval switch
            {
                > 2000 => 2000,
                < 100 => 100,
                _ => value
            };
        }
    } 
    public SensorType Type { get; init; } 
}