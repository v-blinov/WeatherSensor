using System.Collections.Concurrent;
using Weather.ProcessingService.Services.Interfaces;
using Weather.SensorService;

namespace Weather.ProcessingService.Services;

public class LocalRequestQueueService : ILocalRequestQueueService
{
    private ConcurrentQueue<ClientRequest> _queue = new ();

    public void Enqueue(ClientRequest request)
    {
        _queue.Enqueue(request);
    }

    public ClientRequest? TryDequeue()
    {
        _queue.TryDequeue(out var request);
        return request;
    }
}
