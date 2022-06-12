using System.Collections.Concurrent;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ObserverLibrary.Interfaces;
using ObserverLibrary.Models;
using Weather.SensorService.BL.Services.Interfaces;
using Weather.SensorService.BL.Storages.Interfaces;

namespace Weather.SensorService.GrpcServices;

public class GeneratorService : Generator.GeneratorBase, ISubscriber
{
    private readonly ILogger<GeneratorService> _logger;
    private readonly ISensorService _sensorService;
    
    // TODO : нарушение слоев
    private readonly ISensorStorage _sensorStorage;

    private ConcurrentQueue<Event> _events = new();

    public GeneratorService(ILogger<GeneratorService> logger, ISensorService sensorService, ISensorStorage sensorStorage)
    {
        _logger = logger;
        _sensorService = sensorService;
        _sensorStorage = sensorStorage;
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

    public void UpdateSensorEventsQueue(Event @event)
    {
        _events.Enqueue(@event);
    }

    private async Task SubscriberRequestHandlingAsync(IAsyncStreamReader<ClientRequest> requestStream, ServerCallContext context)
    {
        var connectionId = context.GetHttpContext();
        
        while(await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            var request = requestStream.Current;
            _logger.LogInformation("Client {Client} try {Operation} to(of) {SensorId}", connectionId, request.Operation, request.SensorId);

            try
            {
                if(request.Operation == Operation.Subscribe)
                    _sensorService.TrySubscribe(this, new Guid(request.SensorId));
                else
                    _sensorService.TryUnsubscribe(this, new Guid(request.SensorId));

                _logger.LogInformation("Client {Client} success {Operation} to(of) {SensorId}", connectionId, request.Operation, request.SensorId);
            }
            catch(Exception ex)
            {
                _logger.LogError("Client {Client} fail with trying to {Operation} to(of) {SensorId}", connectionId, request.Operation, request.SensorId);
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task PublisherEventSendingAsync(IServerStreamWriter<ServerResponse> responseStream, ServerCallContext context)
    {
        var connectionId = context.GetHttpContext();
        var tasks = _sensorStorage.GetSensors()
                                  .Select(sensor => sensor.StreamGeneration(context.CancellationToken))
                                  .ToList();

        while(!context.CancellationToken.IsCancellationRequested)
        {
            if(!_events.TryDequeue(out var sensorEvent))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                continue;
            }

            _logger.LogInformation("Event from sensor id:{SensorId} for client id:{Client}", sensorEvent.SensorId, connectionId);
            var response = new ServerResponse
            {
                SensorId = sensorEvent.SensorId.ToString(),
                CreatedAt = sensorEvent.CreatedAt.ToTimestamp(),
                Temperature = sensorEvent.Temperature,
                AirHumidity = sensorEvent.AirHumidity,
                Co2 = sensorEvent.Co2,
            };

            await responseStream.WriteAsync(response, context.CancellationToken);
        }
    }
}
