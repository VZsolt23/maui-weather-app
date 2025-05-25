using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiWeatherApp.Models;
using MauiWeatherApp.Services;

namespace MauiWeatherApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly WeatherApiService _weatherApiService;

    public MainViewModel(WeatherApiService weatherApiService)
    {
        _weatherApiService = weatherApiService;
    }

    [ObservableProperty] private string city;
    [ObservableProperty] private string weatherDescription;
    [ObservableProperty] private double temperature;
    [ObservableProperty] private double humidity;
    [ObservableProperty] private TimeOnly sunrise;
    [ObservableProperty] private TimeOnly sunset;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private ImageSource weatherIcon;
    
    public async Task GetWeatherAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Hiba", "A helymeghatározáshoz engedély szükséges!", "OK");
                return;
            }

            Location? location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (location == null)
                location = await GetCurrentLocationAsync();

            if (location != null)
            {
                double latitude = location.Latitude;
                double longitude = location.Longitude;

                var weatherResponse = await _weatherApiService.GetCurrentWeather(latitude, longitude);
                if (weatherResponse is null)
                    return;

                City = weatherResponse.Name;
                WeatherDescription = weatherResponse.Weather[0].Description;
                Temperature = weatherResponse.Main.Temp;
                Humidity = weatherResponse.Main.Humidity;
                
                var time = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunrise);
                var localTime = time.LocalDateTime;
                Sunrise = TimeOnly.FromDateTime(localTime);
                
                time = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunset);
                localTime = time.LocalDateTime;
                Sunset = TimeOnly.FromDateTime(localTime);

                WeatherIcon =
                    ImageSource.FromUri(
                        new Uri($"https://openweathermap.org/img/wn/{weatherResponse.Weather[0].Icon}@2x.png"));
            }
        }
        catch (FeatureNotSupportedException)
        {
            // A készülék nem támogatja a helymeghatározást
            await Shell.Current.DisplayAlert("Hiba",
                "Az eszköz nem támogatja a helymeghatározást.", "OK");
        }
        catch (FeatureNotEnabledException)
        {
            // A GPS ki van kapcsolva
            await Shell.Current.DisplayAlert("Hiba",
                "Kérem, kapcsolja be a helymeghatározást!", "OK");
        }
        catch (PermissionException)
        {
            // Nincs engedély
            await Shell.Current.DisplayAlert("Hiba",
                "Helymeghatározási engedély szükséges!", "OK");
        }
        catch (Exception ex)
        {
            // Egyéb hibák
            await Shell.Current.DisplayAlert("Hiba",
                $"Hiba történt: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task RefreshPage()
    {
        IsRefreshing = true;
        await GetWeatherAsync();
        IsRefreshing = false;
    }

    private static async Task<Location?> GetCurrentLocationAsync()
    {
        // Többszöri próbálkozás implementálása
        const int maxAttempts = 3;
        for (int i = 0; i < maxAttempts; i++)
        {
            try
            {
                GeolocationRequest request =
                    new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location != null)
                    return location;

                await Task.Delay(1000); // Várunk 1 másodpercet az újrapróbálkozás előtt
            }
            catch (Exception)
            {
                if (i == maxAttempts - 1)
                    throw;
            }
        }

        return null;
    }
}