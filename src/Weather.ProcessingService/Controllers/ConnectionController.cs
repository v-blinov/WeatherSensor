using Microsoft.AspNetCore.Mvc;
using Weather.SensorService;

namespace Weather.ProcessingService.Controllers;

[Route("connection")]
public class ConnectionController: Controller
{
    private readonly ILogger<ConnectionController> _logger;
    private readonly Generator.GeneratorClient _generator;

    public ConnectionController(ILogger<ConnectionController> logger, Generator.GeneratorClient generator)
    {
        _logger = logger;
        _generator = generator;
    }

    [HttpGet]
    public ActionResult Subscribe([FromQuery] Guid sensorId, [FromQuery] Operation operation)
    {
        try
        {
            _logger.LogInformation("Try to {Operation} to sensor with id:{SensorId}", operation, sensorId);
            var request = new ClientRequest { SensorId = sensorId.ToString(), Operation = operation };
         
            // не работает..
            // var eventsk = _generator.SendEvents(request);
            // _generator.SendEvents();
            return Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError("[Internal Server Error]: {Error}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
