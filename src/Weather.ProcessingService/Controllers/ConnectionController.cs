using Microsoft.AspNetCore.Mvc;
using Weather.ProcessingService.Services.Interfaces;
using Weather.SensorService;

namespace Weather.ProcessingService.Controllers;

[Route("connection")]
public class ConnectionController: Controller
{
    private readonly ILogger<ConnectionController> _logger;
    private readonly ILocalRequestQueueService _localRequestQueueService;

    public ConnectionController(ILogger<ConnectionController> logger, ILocalRequestQueueService localRequestQueueService)
    {
        _logger = logger;
        _localRequestQueueService = localRequestQueueService;
    }

    [HttpGet]
    public async Task<ActionResult> Subscribe([FromQuery] Guid sensorId, [FromQuery] Operation operation)
    {
        try
        {
            _logger.LogInformation("Try to {Operation} to sensor with id:{SensorId}", operation, sensorId);
            var request = new ClientRequest { SensorId = sensorId.ToString(), Operation = operation };
            
            await _localRequestQueueService.Enqueue(request);

            return Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[Internal Server Error]");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
