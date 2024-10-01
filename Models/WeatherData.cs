using System;

namespace WeatherForecast.Models;

public class WeatherData
{   public string Source { get; set; }
    public string WeatherDescription { get; set; }
    public decimal TemperatureC { get; set; }
}
