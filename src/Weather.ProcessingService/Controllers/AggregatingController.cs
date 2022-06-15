using Microsoft.AspNetCore.Mvc;
using Weather.ProcessingService.BL.Models.Dtos;
using Weather.ProcessingService.BL.Services.Interfaces;

namespace Weather.ProcessingService.Controllers;

[Route("aggregating")]
public class AggregatingController : Controller
{
    private readonly ILogger<AggregatingController> _logger;
    private readonly IAggregatingService _aggregatingService;

    public AggregatingController(ILogger<AggregatingController> logger, IAggregatingService aggregatingService)
    {
        _logger = logger;
        _aggregatingService = aggregatingService;
    }
    
    [HttpGet]
    public ActionResult<Dictionary<Guid, IEnumerable<EventDto>>> GetAggregationByInterval([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        try
        {
            _logger.LogInformation("Try get aggregation for all sensors, From={From}, To={To}", from, to);

            var aggregatingData = _aggregatingService.GetAggregatedDataForInterval(from, to);
            return Ok(aggregatingData);
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
    
    [HttpGet("duration")]
    public ActionResult<Dictionary<Guid, IEnumerable<EventDto>>> GetAggregationByInterval([FromQuery] DateTime from, [FromQuery] int minutesDuration)
    {
        try
        {
            _logger.LogInformation("Try get aggregation for all sensors, From = {From}, Duration = {Duration} minutes", from, minutesDuration);

            var aggregatingData = _aggregatingService.GetAggregatedDataForInterval(from, minutesDuration);
            return Ok(aggregatingData);
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

    [HttpGet("{sensorId:guid}")]
    public ActionResult<Dictionary<Guid, IEnumerable<EventDto>>> GetAggregationByInterval([FromQuery] DateTime from, [FromQuery] DateTime to, Guid sensorId)
    {
        try
        {
            _logger.LogInformation("Try get aggregation with sensorId:{SensorId}, From={From}, To={To}", sensorId, from, to);

            var aggregatingData = _aggregatingService.GetAggregatedDataForInterval(from, to, sensorId);
            return Ok(aggregatingData);
        }
        catch(KeyNotFoundException ex)
        {
            _logger.LogError(ex, "[Not Found]: {SensorId}", sensorId);
            return NotFound();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[Internal Server Error]");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpGet("{sensorId:guid}/duration")]
    public ActionResult<Dictionary<Guid, IEnumerable<EventDto>>> GetAggregationByInterval([FromQuery] DateTime from, [FromQuery] int minutesDuration, Guid sensorId)
    {
        try
        {
            _logger.LogInformation("Try get aggregation with sensorId: {SensorId}, From = {From}, Duration = {Duration} minutes", sensorId, from, minutesDuration);

            var aggregatingData = _aggregatingService.GetAggregatedDataForInterval(from, minutesDuration, sensorId);
            return Ok(aggregatingData);
        }
        catch(KeyNotFoundException ex)
        {
            _logger.LogError(ex, "[Not Found]: {SensorId}", sensorId);
            return NotFound();
        }
        catch(ArgumentException ex)
        {
            _logger.LogError(ex, "[Argument Exception]");
            return BadRequest();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[Internal Server Error]");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
