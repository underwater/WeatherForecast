using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherForecast.Interfaces;
using WeatherForecast.Providers;
using WeatherForecast.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

// Register configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Register HttpClient with retry policy
builder.Services.AddHttpClient("WeatherClient")
    .AddPolicyHandler(retryPolicy);

// Register weather providers
builder.Services.AddScoped<IWeatherProvider, TomorrowIoProvider>();
builder.Services.AddScoped<IWeatherProvider, WeatherstackProvider>();
builder.Services.AddScoped<IWeatherProvider, VisualCrossingProvider>();

// Register the concrete WeatherService
builder.Services.AddScoped<WeatherService>();

// Register the caching decorator as IWeatherService
builder.Services.AddScoped<IWeatherService>(provider =>
{
    var weatherService = provider.GetRequiredService<WeatherService>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CachingWeatherServiceDecorator(weatherService, cache);
});

var app = builder.Build();

// normally we would use the following to configure swagger in development
// but in this demo, I leave it to verify the swagger is working in production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast API v1");
    c.RoutePrefix = string.Empty; // Swagger UI at the app's root
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
