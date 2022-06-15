using System.Threading.Channels;
using Weather.ProcessingService.Services.Interfaces;
using Weather.SensorService;

namespace Weather.ProcessingService.Services;

public class LocalRequestQueueService : ILocalRequestQueueService
{
    private static readonly BoundedChannelOptions Params = new(1)
    {
        SingleReader = true,
        SingleWriter = true,
        FullMode = BoundedChannelFullMode.Wait
    };

    private readonly Channel<ClientRequest> _queue = Channel.CreateBounded<ClientRequest>(Params);

    public ValueTask Enqueue(ClientRequest request)
    {
        return _queue.Writer.WriteAsync(request);
    }

    public ValueTask<ClientRequest> Dequeue(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAsync(cancellationToken);
    }
}
