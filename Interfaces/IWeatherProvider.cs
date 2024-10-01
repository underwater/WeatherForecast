using System;
using System.Threading.Tasks;
using WeatherForecast.Models;

namespace WeatherForecast.Interfaces;

public interface IWeatherProvider
{
    string ProviderName { get; }
    Task<WeatherData> GetForecastAsync(DateTime date, string city, string country);
}
