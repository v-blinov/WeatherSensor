using Weather.SensorService.BL.Models;

namespace Weather.SensorService.BL.Storages.Interfaces;

public interface ISensorStorage
{
    public void Add(Sensor sensor);
    public void AddRange(IEnumerable<Sensor> sensors);
    public Sensor GetSensor(Guid id);
    public Dictionary<Guid, Sensor> GetSensor(IEnumerable<Guid> ids);
    public IEnumerable<Guid> GetSensorIds();
}
