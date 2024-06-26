using System.Text.Json;

namespace cycling_map;

public class RouteParser : IParser
{
    public T JsonParse<T>(string json)
    {
        try
        {
            // Deserialize the JSON string into a GeocodeResponse object
            var routeData = JsonSerializer.Deserialize<RouteJson>(json);

            return (T)Convert.ChangeType(routeData.Routes[0], typeof(T));
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse geocode JSON: {ex.Message}");
        }
    }
}