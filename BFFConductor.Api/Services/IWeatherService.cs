using BFFConductor.Models;

namespace BFFConductor.Api.Services;

public interface IWeatherService
{
    OperationResult<IEnumerable<WeatherForecast>> GetForecasts();
    OperationResult<WeatherForecast> GetForecastById(int id);
}
