using Grpc.Core;
using Weather.ProcessingService.BL.Models.Dtos;
using Weather.ProcessingService.BL.Services.Interfaces;
using Weather.ProcessingService.Services.Interfaces;
using Weather.SensorService;

namespace Weather.ProcessingService.HostedServices;

public class StreamReaderHostedService : BackgroundService
{
    private readonly ILogger<StreamReaderHostedService> _logger;
    private readonly IEventService _eventService;
    private readonly IServiceProvider _provider;
    private readonly ILocalRequestQueueService _localRequestQueue;

    public StreamReaderHostedService(
        ILogger<StreamReaderHostedService> logger,
        IEventService eventService,
        IServiceProvider provider,
        ILocalRequestQueueService localRequestQueue)
    {
        _logger = logger;
        _eventService = eventService;
        _provider = provider;
        _localRequestQueue = localRequestQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<Generator.GeneratorClient>();
        using var eventResponseStream = client.SendEvents(new Metadata(), cancellationToken: stoppingToken);

        var responseReaderTask = ResponseReaderTask(stoppingToken, eventResponseStream);
        var sendRequestTask = SendRequestTask(eventResponseStream, stoppingToken);

        await Task.WhenAll(sendRequestTask, responseReaderTask); 
        await eventResponseStream.RequestStream.CompleteAsync();
        
        _logger.LogInformation("{Worker} stop executing", nameof(StreamReaderHostedService));
    }

    private async Task SendRequestTask(AsyncDuplexStreamingCall<ClientRequest, ServerResponse> eventResponseStream, CancellationToken stoppingToken)
    {
        try
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var @event = _localRequestQueue.TryDequeue();
                if(@event is null)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }

                await eventResponseStream.RequestStream.WriteAsync(@event, stoppingToken);
            }
        }
        catch(RpcException ex)
        {
            _logger.LogError(ex, "Rpc exception");
            // TODO: обработать ошибку
        }
    }

    private async Task ResponseReaderTask(CancellationToken stoppingToken, AsyncDuplexStreamingCall<ClientRequest, ServerResponse> eventResponseStream)
    {
        try
        {
            var responseTask = Task.Run(async () =>
            {
                while(await eventResponseStream.ResponseStream.MoveNext(stoppingToken))
                {
                    var e = eventResponseStream.ResponseStream.Current;

                    var @event = new EventDto { CreatedAt = e.CreatedAt.ToDateTime(), Temperature = e.Temperature, AirHumidity = e.AirHumidity, Co2 = e.Co2 };

                    var sensorId = new Guid(e.SensorId);
                    _eventService.Add(sensorId, @event);
                }
            });

            await responseTask;
        }
        catch(RpcException ex)
        {
            _logger.LogError(ex, "Rpc exception");
            // TODO: обработать ошибку
        }
    }
}
