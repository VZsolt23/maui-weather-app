using MauiWeatherApp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MauiWeatherApp.Services;

public class WeatherApiService
{
    private readonly IConfiguration _configuration;

    public WeatherApiService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<WeatherResponse?> GetCurrentWeather(double latitude, double longitude)
    {
        var apikey = _configuration.GetValue<string>("WeatherApiKey");
        if (string.IsNullOrWhiteSpace(apikey))
        {
            // todo: await Shell.Current.DisplayAlert("Hiba", "Hiányzó API kulcs!", "OK");
            return null;
        }

        var httpClient = new HttpClient();
        var response =
            await httpClient.GetStringAsync(
                $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apikey}&units=metric&lang=hu");

        return JsonConvert.DeserializeObject<WeatherResponse>(response);
    }
}