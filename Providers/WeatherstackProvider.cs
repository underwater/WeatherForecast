using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Providers;

public class WeatherstackProvider : IWeatherProvider
{
    public string ProviderName => "Weatherstack";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherstackProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["WeatherstackApiKey"];
    }

    public async Task<WeatherData> GetForecastAsync(DateTime date, string city, string country)
    {
        var endpoint = $"http://api.weatherstack.com/current?access_key={_apiKey}&query={city},{country}";

        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            // Handle error
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var contentAsJson = JObject.Parse(content);

        var currentWeather = contentAsJson["current"];
        if (currentWeather == null)
        {
            return null;
        }

        var forecast = new WeatherData
        {
            Source = ProviderName,
            WeatherDescription = currentWeather["weather_descriptions"]?[0]?.ToString(),
            TemperatureC = currentWeather["temperature"]?.Value<decimal>() ?? 0
        };

        return forecast;
    }
}
