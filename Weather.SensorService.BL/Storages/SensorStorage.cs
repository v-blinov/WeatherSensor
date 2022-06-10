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
        var sensor = _storage.FirstOrDefault(p => p.Id == id);
        if (sensor is null)
            throw new KeyNotFoundException(id.ToString());
        
        return sensor;
    }

    public IEnumerable<Sensor> GetSensors()
    {
        return _storage.ToArray();
    }
    
    public IEnumerable<Sensor> GetSensors(IEnumerable<Guid> ids)
    {
        if(!ids.Any())
            return Enumerable.Empty<Sensor>();
        
        var sensors = new List<Sensor>(ids.Count());
        foreach(var id in ids)
        {
            var sensor = _storage.FirstOrDefault(p => p.Id == id);
            if (sensor is null)
                throw new KeyNotFoundException(id.ToString());
            
            sensors.Add(sensor);
        } 

        return sensors;
    }

    public IEnumerable<Guid> GetSensorIds()
    {
        return _storage.Select(p => p.Id).ToArray();
    }
}
