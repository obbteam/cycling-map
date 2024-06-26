using System.Text.Json;

namespace cycling_map;

public class RouteParser
{
    public static Route ParseRoute(string json)
    {
        try
        {
            List<Location> RoutePoints = new List<Location>();
            // Deserialize the JSON string into a GeocodeResponse object
            Route routeData = JsonSerializer.Deserialize<Route>(json);

            return routeData;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse geocode JSON: {ex.Message}");
        }
    }
}