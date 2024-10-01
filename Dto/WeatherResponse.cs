using System;
using System.Collections.Generic;
using WeatherForecast.Models;

public record WeatherResponse(DateTime Date, string City, string Country, IEnumerable<WeatherData> Forecasts);