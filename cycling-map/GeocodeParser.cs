using System.Text.Json;
using System.Windows;

namespace cycling_map;

public static class GeocodeParser
{
    public static Location ParseGeocode(string json)
    {
        try
        {
            // Deserialize the JSON string into a GeocodeResponse object
            var geocodeResponse = JsonSerializer.Deserialize<GeoJson.GeocodeResponse>(json);

            if (geocodeResponse == null || geocodeResponse.results == null || geocodeResponse.results.Count == 0)
            {
                throw new Exception("No geocode results found.");
            }

            // Assuming we take the first result
            var result = geocodeResponse.results[0];

            // Extract latitude and longitude from the position object
            double latitude = result.position.lat;
            double longitude = result.position.lon;

            // Create and return a new Location object
            return new Location(latitude, longitude);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to parse geocode JSON: {ex.Message}");
            throw;
        }
    }
}