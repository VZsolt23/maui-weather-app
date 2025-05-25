namespace MauiWeatherApp.Models;

public class WeatherResponse
{
    public CoordDto Coord { get; set; } = new();
    public List<WeatherDto> Weather { get; set; } = [];
    public string Base { get; set; } = string.Empty;
    public MainDto Main { get; set; } = new();
    public int Visibility { get; set; }
    public WindDto Wind { get; set; } = new();
    public RainDto Rain { get; set; } = new();
    public CloudsDto Clouds { get; set; } = new();
    public long Dt { get; set; }
    public Sys Sys { get; set; } = new();
    public int Timezone { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Cod { get; set; }
}