using System.Text.Json.Serialization;

namespace MauiWeatherApp.Models;

public class RainDto
{
    [JsonPropertyName("1h")] 
    public double OneHour { get; set; }
}