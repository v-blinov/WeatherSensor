using System.Collections.Concurrent;
using Grpc.Core;

namespace Weather.SensorService.GrpcServices;

public class GeneratorService : Generator.GeneratorBase
{
    private readonly ILogger<GeneratorService> _logger;
    private ConcurrentQueue<ServerResponse> _events = new();

    public GeneratorService(ILogger<GeneratorService> logger)
    {
        _logger = logger;
    }
    
    public override async Task SendEvents(
        IAsyncStreamReader<ClientRequest> requestStream, 
        IServerStreamWriter<ServerResponse> responseStream, 
        ServerCallContext context)
    {
        try
        {
            var subscriberTask = SubscriberRequestHandlingAsync(requestStream, context);
            var publisherTask = PublisherEventSendingAsync(responseStream, context);

            await Task.WhenAll(subscriberTask, publisherTask);
        }
        catch(OperationCanceledException)
        {
            _logger.LogWarning("A operation was canceled");
        }
    }

    private async Task SubscriberRequestHandlingAsync(IAsyncStreamReader<ClientRequest> requestStream, ServerCallContext context)
    {
        var connectionId = context.GetHttpContext();
        
        while(await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            var request = requestStream.Current;
            
            _logger.LogInformation("Client {Client} try {Operation} to(of) {SensorId}", connectionId, request.Operation, request.SensorId);
            
            // subscribe / unsibscribe
        }
    }
    
    private async Task PublisherEventSendingAsync(IServerStreamWriter<ServerResponse> responseStream, ServerCallContext context)
    {
        var connectionId = context.GetHttpContext();
        // пройтись по всем сенсорам, запустить метод генерации и получить список задач
        
        while(!context.CancellationToken.IsCancellationRequested)
        {
            // складывать события в ConcurrentQueue и отдавать в response
            // WhenAny()

            var response = new ServerResponse(); // TODO
            
            _logger.LogInformation("Event from sensor id:{SensorId} for client id:{Client}", response.SensorId, connectionId);
            await responseStream.WriteAsync(response);
        }
    }
}
