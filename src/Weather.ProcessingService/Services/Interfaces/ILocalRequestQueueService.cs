using Weather.SensorService;

namespace Weather.ProcessingService.Services.Interfaces;

public interface ILocalRequestQueueService
{
    ValueTask Enqueue(ClientRequest request);
    ValueTask<ClientRequest> Dequeue(CancellationToken cancellationToken);
}
