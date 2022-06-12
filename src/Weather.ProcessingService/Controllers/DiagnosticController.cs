using Microsoft.AspNetCore.Mvc;
using Weather.ProcessingService.BL.Models.Dtos;
using Weather.ProcessingService.BL.Services.Interfaces;

namespace Weather.ProcessingService.Controllers;

[Route("diagnostic")]
public class DiagnosticController: Controller
{
    private readonly ILogger<AggregatingController> _logger;
    private readonly IAggregatingService _aggregatingService;

    public DiagnosticController(ILogger<AggregatingController> logger, IAggregatingService aggregatingService)
    {
        _logger = logger;
        _aggregatingService = aggregatingService;
    }
    
    [HttpGet]
    public ActionResult<Dictionary<Guid, IEnumerable<EventDto>>> GetDiagnostic()
    {
        try
        {
            _logger.LogInformation("Try get diagnostic");

            var aggregatingData = _aggregatingService.GetAllAggregatedItems();
            return Ok(aggregatingData);
        }
        catch(Exception ex)
        {
            _logger.LogError("[Internal Server Error]: {Error}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
