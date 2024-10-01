using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Services;

public class WeatherService : IWeatherService
{
    private readonly IEnumerable<IWeatherProvider> _weatherProviders;

    public WeatherService(IEnumerable<IWeatherProvider> weatherProviders)
    {
        _weatherProviders = weatherProviders;
    }


    public async Task<IEnumerable<WeatherData>> GetWeatherAsync(DateTime date, string city, string country)
    {
        var tasks = new List<Task<WeatherData>>();

        foreach (var provider in _weatherProviders)
        {
            tasks.Add(provider.GetForecastAsync(date, city, country));
        }

        var results = await Task.WhenAll(tasks);

        // Filter out any null results (in case a provider fails)
        var forecasts = new List<WeatherData>();
        foreach (var result in results)
        {
            if (result != null)
            {
                forecasts.Add(result);
            }
        }

        return forecasts;
    }
}
