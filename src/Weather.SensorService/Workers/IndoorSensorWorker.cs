using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Weather.SensorService.Models;
using Weather.SensorService.Workers.Interfaces;

namespace Weather.SensorService.Workers;

public class IndoorSensorWorker : Generator.GeneratorBase, ISensorWorker
{
    private readonly ILogger<IndoorSensorWorker> _logger;

    private readonly Guid _id;
    private readonly int _delayPeriod;
    private readonly Generator.GeneratorClient _client;
    private State _state;

    
    public IndoorSensorWorker(GrpcClientFactory grpcClientFactory, ILogger<IndoorSensorWorker> logger)
    {
        _id = Guid.NewGuid();
        
        _state = new State(); // TODO: Считывать из конфига
        _delayPeriod = 1000;  // TODO: Считывать из конфига
        _client = grpcClientFactory.CreateClient<Generator.GeneratorClient>("IndoorSensorWorker");
        
        _logger = logger;
    }

    public override async Task SendEvents(
        IAsyncStreamReader<ClientRequest> requestStream,
        IServerStreamWriter<ServerResponse> responseStream,
        ServerCallContext context)
    {
        try
        {
            var httpContext = context.GetHttpContext ();
            _logger.LogInformation ("Connection id: {ConnectionId}", httpContext.Connection.Id);

            if(!await requestStream.MoveNext()) 
                return;
            
            var random = new Random();
            
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_delayPeriod, context.CancellationToken);
                _state = GenerateState(random);

                var response = new ServerResponse
                {
                    SensorId = _id.ToString(),
                    CreatedAt = DateTime.Now.ToTimestamp(),
                    Temperature = _state.Temperature,
                    AirHumidity = _state.AirHumidity,
                    Co2 = _state.Co2
                };
                
                await responseStream.WriteAsync(response, context.CancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("An operation was canceled");
        }
    }

    public State GenerateState(Random random)
    {
        return new State
        {
            Temperature = _state.Temperature + (random.Next(-2, 2) * 0.1), 
            AirHumidity = _state.AirHumidity + random.Next(-2, 2), 
            Co2 = _state.Co2 + random.Next(-70, 70)
        };
    }
    
    // Пытался сделать сенсоры отдельными воркерами
    // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    // {
    //     await using var scope = _provider.CreateAsyncScope();
    //     var client = scope.ServiceProvider.GetRequiredService<Generator.GeneratorBase>();
    //     using var sendEvents = client.SendEvents(, cancellationToken: stoppingToken);
    //     
    //     while(!stoppingToken.IsCancellationRequested)
    //     {
    //         await Task.Delay(_delayPeriod, stoppingToken);
    //
    //         _logger.LogInformation("{Worker} work", nameof(IndoorSensorWorker));
    //
    //         State = GenerateState();
    //
    //         await responseStream.WriteAsync(result, context.CancellationToken);
    //     }
    // }
}
