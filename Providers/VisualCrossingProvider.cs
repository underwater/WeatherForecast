
    using System;
using System.Linq;
using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    using WeatherForecast.Interfaces;
    using WeatherForecast.Models;

    namespace WeatherForecast.Providers;

// https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/zurich?unitGroup=metric&include=days&key=YRJQ3W6BML45QJQUEQA3QY4DJ&contentType=json
public class VisualCrossingProvider : IWeatherProvider
{
    public string ProviderName => "VisualCrossing";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _weatherApiUrl;

    public VisualCrossingProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["VisualCrossingApiKey"];
        _weatherApiUrl = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline";
    }

    // please create a funtion that formats a datetime as yyyy-mm-dd
    private string Format(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }
    public async Task<WeatherData> GetForecastAsync(DateTime date, string city, string country)
    {
        var endpoint = $"{_weatherApiUrl}/{city}/{Format(date)}/{Format(date)}?unitGroup=metric&include=days&key={_apiKey}&contentType=json";

        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            // Handle error
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var contentAsJson = JObject.Parse(content);

        var temperature = contentAsJson["days"]?[0]?["temp"].ToString();
        var description = contentAsJson["days"]?[0]?["description"].ToString();


        var forecast = new WeatherData
        {
            Source = ProviderName,
            WeatherDescription = description,
            TemperatureC = decimal.Parse(temperature)
        };

        return forecast;
    }
}

