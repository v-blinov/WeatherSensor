using Weather.SensorService.Models;
using Weather.SensorService.Workers.Interfaces;

namespace Weather.SensorService.Workers;

public class OutdoorSensorWorker : BackgroundService, ISensorWorker
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => throw new NotImplementedException();
    public State GenerateState(Random random) => throw new NotImplementedException();
}
