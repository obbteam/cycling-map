using System.Text.Json;

namespace cycling_map;

public class RouteParser
{
    public static List<Location> ParseRoute(string json, Summary responseSummary)
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
            responseSummary.LengthInMeters = routeData.Routes[0].Legs[0].Summary.LengthInMeters;
            responseSummary.DepartureTime = routeData.Routes[0].Legs[0].Summary.DepartureTime;
            responseSummary.ArrivalTime = routeData.Routes[0].Legs[0].Summary.ArrivalTime;

            return RoutePoints;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse geocode JSON: {ex.Message}");
        }
    }
}