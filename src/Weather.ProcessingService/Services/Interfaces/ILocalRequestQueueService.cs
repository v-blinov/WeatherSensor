using Weather.SensorService;

namespace Weather.ProcessingService.Services.Interfaces;

public interface ILocalRequestQueueService
{
    void Enqueue(ClientRequest request);
    ClientRequest? TryDequeue();
}
