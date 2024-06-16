using System.Text.Json;

namespace cycling_map;

public class RouteParser
{
    public static List<Location> ParseRoute(string json)
    {
        try
        {
            List<Location> RoutePoints = new List<Location>();
            // Deserialize the JSON string into a GeocodeResponse object
            var routeData = JsonSerializer.Deserialize<RouteJson>(json);

            foreach (var point in routeData.Routes[0].Legs[0].Points)
            {
                RoutePoints.Add(new Location(point.Latitude, point.Longitude));
            }

            return RoutePoints;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse geocode JSON: {ex.Message}");
        }
    }
}