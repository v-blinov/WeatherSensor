using Weather.SensorService.BL.Enums;

namespace Weather.SensorService.BL.Models.Dtos;

public record SensorSettingDto
{
    public int WorkInterval { get; init; } 
    public SensorType Type { get; init; }   // TODO : parser from enum value to string
}
