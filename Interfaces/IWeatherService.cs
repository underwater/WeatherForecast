using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherForecast.Models;
namespace WeatherForecast.Interfaces;

public interface IWeatherService
{
    public Task<IEnumerable<WeatherData>> GetWeatherAsync(DateTime date, string city, string country);
}
