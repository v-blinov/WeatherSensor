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

    private static TimeSpan backoff = TimeSpan.FromSeconds(1);
    private static TimeSpan maxBackoff = TimeSpan.FromSeconds(5);
    const double backoffMultiplier = 1.5;
    
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
        
        await DoWork(stoppingToken, client, null, null);

        _logger.LogInformation("{Worker} stop executing", nameof(StreamReaderHostedService));
    }

    private async Task DoWork(CancellationToken stoppingToken, Generator.GeneratorClient client, Task? responseReaderTask, Task? sendRequestTask)
    {
        try
        {
            using var eventResponseStream = client.SendEvents(new Metadata(), cancellationToken: stoppingToken);

            responseReaderTask ??= ResponseReaderTask(eventResponseStream, stoppingToken);
            sendRequestTask ??= SendRequestTask(eventResponseStream, stoppingToken);

            await Task.WhenAll(sendRequestTask, responseReaderTask);
            await eventResponseStream.RequestStream.CompleteAsync();
        }
        catch(RpcException ex)
        {
            // Попытка реализовать retry
            // задачи завершились из-за необработанного исключения
            if(ex.Status.StatusCode == StatusCode.Unavailable)
            {
                await Task.Delay(backoff);
                if(backoff >= maxBackoff || backoff * backoffMultiplier >= maxBackoff)
                    backoff *= backoffMultiplier;
                
                _logger.LogInformation("Attempt to retry");
                
                await DoWork(stoppingToken, client, responseReaderTask, sendRequestTask);
            }

            _logger.LogError(ex, "");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "");
        }
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
            throw;
        }
    }

    private async Task ResponseReaderTask(AsyncDuplexStreamingCall<ClientRequest, ServerResponse> eventResponseStream, CancellationToken stoppingToken)
    {
        try
        {
            var responseTask = Task.Run(async () =>
            {
                while(await eventResponseStream.ResponseStream.MoveNext(stoppingToken))
                {
                    var e = eventResponseStream.ResponseStream.Current;

                    var @event = new EventDto
                    {
                        CreatedAt = e.CreatedAt.ToDateTime(), 
                        Temperature = e.Temperature, 
                        AirHumidity = e.AirHumidity, 
                        Co2 = e.Co2
                    };

                    var sensorId = new Guid(e.SensorId);
                    _eventService.Add(sensorId, @event);
                }
            }, stoppingToken);

            await responseTask;
        }
        catch(RpcException ex)
        {
            _logger.LogError(ex, "Rpc exception");
            throw new Exception();
        }
    }
}
