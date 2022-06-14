using Microsoft.AspNetCore.Mvc;
using Weather.SensorService.BL.Models.Dtos;
using Weather.SensorService.BL.Services.Interfaces;

namespace Weather.SensorService.Controllers;

[Route("sensor")]
public class SensorController : Controller
{
    private readonly ILogger<SensorController> _logger;
    private readonly ISensorService _sensorService;

    public SensorController(ILogger<SensorController> logger, ISensorService sensorService)
    {
        _logger = logger;
        _sensorService = sensorService;
    }

    [HttpGet("sensors")]
    public ActionResult<IEnumerable<string>> GetSensors()
    {
        try
        {
            _logger.LogInformation("Try get data about sensors");
            var sensors = _sensorService.GetSensors();
            return Ok(sensors);
        }
        catch(KeyNotFoundException ex)
        {
            _logger.LogError(ex, "[Not Found]");
            return NotFound();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[Internal Server Error]");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpGet("{id:Guid}")]
    public ActionResult<EventDto> GetSensor(Guid id)
    {
        try
        {
            _logger.LogInformation("Try get sensor with id = {SensorId}", id);
            var sensor = _sensorService.GetSensor(id);
            return Ok(sensor);
        }
        catch(KeyNotFoundException ex)
        {
            _logger.LogError(ex, "[Not Found]: {SensorId}", id);
            return NotFound();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[Internal Server Error]");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}