using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Weather.SensorService.BL.Models.Dtos;
using Weather.SensorService.BL.Services.Interfaces;
using Weather.SensorService.Models;

namespace Weather.SensorService.Controllers;

[Route("sensor")]
public class SensorController : Controller
{
    private readonly List<SensorSettings> _options;
    
    private readonly ILogger<SensorController> _logger;
    private readonly ISensorService _sensorService;

    public SensorController(IOptions<List<SensorSettings>> options, ILogger<SensorController> logger, ISensorService sensorService)
    {
        _options = options.Value;
        _logger = logger;
        _sensorService = sensorService;
    }

    [HttpGet("initialize")]
    public ActionResult<IEnumerable<SensorDto>> Initialize()
    {
        try
        {
            _logger.LogInformation("Try initialize sensors");

            var sensorsSettings = _options;
            if(sensorsSettings is null)
            {
                _logger.LogInformation("Initializing data not found");
                return NotFound("Initializing data");
            }

            if(!sensorsSettings.Any())
            {
                _logger.LogInformation("Initializing Data is Empty");
                return Ok("Initializing Data is Empty");
            }

            var sensorDtos = sensorsSettings.Select(p => new SensorDto
            {
                Id = Guid.NewGuid(),
                SensorSettings = new SensorSettingDto
                {
                    Type = p.SensorType, 
                    WorkInterval = p.WorkInterval
                },
                Event = new EventDto
                {
                    CreatedAt = DateTime.UtcNow,
                    EventData = new EventDataDto
                    {
                        Temperature = p.InitializeEventValues.Temperature, 
                        AirHumidity = p.InitializeEventValues.AirHumidity, 
                        Co2 = p.InitializeEventValues.Co2
                    }
                }
            }).ToArray();

            _sensorService.AddRange(sensorDtos);

            return Created("", sensorDtos);
        }
        catch(KeyNotFoundException ex)
        {
            _logger.LogError("[Not Found]: {Error}", ex.Message);
            return NotFound();
        }
        catch(Exception ex)
        {
            _logger.LogError("[Internal Server Error]: {Error}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
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
            _logger.LogError("[Not Found]: {Error}", ex.Message);
            return NotFound();
        }
        catch(Exception ex)
        {
            _logger.LogError("[Internal Server Error]: {Error}", ex.Message);
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
            _logger.LogError("[Not Found]: {SensorId}. {Error}", id, ex.Message);
            return NotFound();
        }
        catch(Exception ex)
        {
            _logger.LogError("[Internal Server Error]: {Error}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    //TODO: [HttpGet("/{id}")]
    //TODO: [HttpGet("/sensor_ds")]
    //TODO: [HttpGet("?{id=}&{id=}")]
}