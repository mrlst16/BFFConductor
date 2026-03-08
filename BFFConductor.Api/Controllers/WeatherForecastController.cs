using BFFConductor.Api.Services;
using BFFConductor.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BFFConductor.Api.Controllers;

// Controller-level overrides: these apply to all actions unless an action overrides them
[ApiController]
[UseBFFResponseFilter]
[Route("[controller]")]
[ErrorDisplay(ErrorCodes.ValidationFailed, DisplayMethod.Modal)]  // spec default is Inline (2); override to Modal here
[ErrorDisplay(ErrorCodes.NotFound, DisplayMethod.Page)]           // spec default is Toast (1); override to Page here
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherForecastController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    // GET /weatherforecast
    // Success  → 200, x-handle-message-as: 1 (Toast, from spec default)
    [HttpGet]
    public IActionResult Get()
    {
        var result = _weatherService.GetForecasts();
        return Ok(result);
    }

    // GET /weatherforecast/{id}
    // Success    → 200, x-handle-message-as: 1 (Toast)
    // id <= 0    → 422, x-handle-message-as: 2 (Inline)  ← action overrides controller's Modal
    // id > 100   → 404, x-handle-message-as: 4 (Page)    ← inherits controller override
    [HttpGet("{id:int}")]
    [ErrorDisplay(ErrorCodes.ValidationFailed, DisplayMethod.Inline)] // action override: Inline instead of controller's Modal
    public IActionResult GetById(int id)
    {
        var result = _weatherService.GetForecastById(id);
        return Ok(result);
    }
}
