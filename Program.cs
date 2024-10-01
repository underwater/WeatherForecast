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


var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1));

var builder = WebApplication.CreateBuilder(args);

// weather API Keys
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});


// Configuration for API keys
builder.Services.AddSingleton(builder.Configuration);

// Register the HttpClient with the retry policy
IHttpClientBuilder httpClientBuilder = builder.Services.AddHttpClient("WeatherClient")
    .AddPolicyHandler(retryPolicy);

// Register multiple weather providers
builder.Services.AddScoped<IWeatherProvider, TomorrowIoProvider>();
builder.Services.AddScoped<IWeatherProvider, WeatherstackProvider>();
builder.Services.AddScoped<IWeatherProvider, VisualCrossingProvider>();


builder.Services.AddScoped<WeatherService>();

// Register the main weather service with caching
builder.Services.AddScoped<IWeatherService>(provider =>
{
    var weatherService = provider.GetRequiredService<WeatherService>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CachingWeatherServiceDecorator(weatherService, cache);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecast API v1"));
    app.UseHttpLogging();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
