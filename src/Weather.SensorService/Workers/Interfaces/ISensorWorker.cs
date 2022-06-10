using ObserverLibrary.Models;
using Weather.SensorService.Models;

namespace Weather.SensorService.Workers.Interfaces;

public interface ISensorWorker
{
    public State GenerateState(Random random);
}
