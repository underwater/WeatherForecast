using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;


namespace WeatherForecast.Providers;

public class TomorrowIoProvider : IWeatherProvider
{
    public string ProviderName => "Tomorrow.io";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public TomorrowIoProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["TomorrowIoApiKey"];
    }

    public async Task<WeatherData> GetForecastAsync(DateTime date, string city, string country)
    {
        // https://docs.tomorrow.io/reference/realtime-weather
        var endpoint = $"https://api.tomorrow.io/v4/weather/realtime?location={city}&apikey={_apiKey}";

        var response = await _httpClient.GetAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();
        var contentAsJson = JObject.Parse(content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var forecast = new WeatherData
        {
            Source = ProviderName,
            WeatherDescription = "not available",
            TemperatureC = GetTemperatureAsync(contentAsJson)
        };

        return forecast;
    }

    private decimal GetTemperatureAsync(JObject contentAsJson) =>
        decimal.Parse(contentAsJson["data"]["values"]["temperature"].ToString());
    
}
