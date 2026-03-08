using BFFConductor.Models;

namespace BFFConductor.Api.Services;

public class WeatherService : IWeatherService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public OperationResult<IEnumerable<WeatherForecast>> GetForecasts()
    {
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });

        return OperationResult<IEnumerable<WeatherForecast>>.Ok(forecasts);
    }

    public OperationResult<WeatherForecast> GetForecastById(int id)
    {
        if (id <= 0)
            return OperationResult<WeatherForecast>.Fail("Invalid forecast ID.", ErrorCodes.ValidationFailed);

        if (id > 100)
            return OperationResult<WeatherForecast>.Fail("Forecast not found.", ErrorCodes.NotFound);

        var forecast = new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(id)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };

        return OperationResult<WeatherForecast>.Ok(forecast);
    }
}
