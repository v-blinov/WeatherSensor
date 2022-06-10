using Weather.SensorService.BL.Models;
using Weather.SensorService.BL.Storages.Interfaces;

namespace Weather.SensorService.BL.Storages;

public class SensorStorage : ISensorStorage
{
    private readonly List<Sensor> _storage = new();

    public void Add(Sensor sensor)
    {
        _storage.Add(sensor);
    }

    public void AddRange(IEnumerable<Sensor> sensors)
    {
        _storage.AddRange(sensors);
    }
    
    public Sensor GetSensor(Guid id)
    {
        _storage.FirstOrDefault(p => p.Id == id)
        
        return _storage.FirstOrDefault(p => p.Id == id);
    }

    public Dictionary<Guid, Sensor?> GetSensor(IEnumerable<Guid> ids)
    {
        if(!ids.Any())
            return new Dictionary<Guid, Sensor?>();
        
        var sensors = new Dictionary<Guid, Sensor?>();
        foreach(var id in ids)
            sensors[id] = _storage.FirstOrDefault(p => p.Id == id);

        return sensors;
    }

    public IEnumerable<Guid> GetSensorIds() => throw new NotImplementedException();

    public IEnumerable<Sensor> GetSensors()
    {
        return _storage;
    }
}
