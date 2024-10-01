using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Services;

public class CachingWeatherServiceDecorator : IWeatherService
{
    private readonly IWeatherService _innerWeatherService;
    private readonly IMemoryCache _cache;

    public CachingWeatherServiceDecorator(IWeatherService innerWeatherService, IMemoryCache cache)
    {
        _innerWeatherService = innerWeatherService;
        _cache = cache;
    }

    public async Task<IEnumerable<WeatherData>> GetWeatherAsync(DateTime date, string city, string country)
    {
        string cacheKey = $"{date:yyyyMMdd}_{city}_{country}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<WeatherData> cachedForecasts))
        {
            return cachedForecasts;
        }

        var forecasts = await _innerWeatherService.GetWeatherAsync(date, city, country);

        _cache.Set(cacheKey, forecasts, TimeSpan.FromHours(1));

        return forecasts;
    }
}
