using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Controllers;

[ApiController]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpPost]
    [Route("weather")]
    public async Task<ActionResult<WeatherResponse>> GetWeather([FromBody] WeatherRequest request)
    {
        try
        {
            var weather = await _weatherService.GetWeatherAsync(request.Date, request.City, request.Country);

            return Ok(new WeatherResponse(request.Date, request.City, request.Country, weather));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the weather request");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }
}
