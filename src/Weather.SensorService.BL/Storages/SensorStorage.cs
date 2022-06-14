using Weather.SensorService.BL.Models.Interfaces;
using Weather.SensorService.BL.Storages.Interfaces;

namespace Weather.SensorService.BL.Storages;

public class SensorStorage : ISensorStorage
{
    private readonly List<ISensor> _storage = new();

    public void Add(ISensor sensor)
    {
        _storage.Add(sensor);
    }

    public void AddRange(IEnumerable<ISensor> sensors)
    {
        _storage.AddRange(sensors);
    }
    
    public ISensor GetSensor(Guid id)
    {
        var sensor = _storage.FirstOrDefault(p => p.Id == id);
        if (sensor is null)
            throw new KeyNotFoundException(id.ToString());
        
        return sensor;
    }

    public IEnumerable<ISensor> GetSensors()
    {
        return _storage.ToArray();
    }
}
