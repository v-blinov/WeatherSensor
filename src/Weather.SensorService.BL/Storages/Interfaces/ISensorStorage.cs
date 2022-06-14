using Weather.SensorService.BL.Models.Interfaces;

namespace Weather.SensorService.BL.Storages.Interfaces;

public interface ISensorStorage
{
    public void Add(ISensor sensor);
    public void AddRange(IEnumerable<ISensor> sensors);
    public ISensor GetSensor(Guid id);
    public IEnumerable<ISensor> GetSensors();
}
