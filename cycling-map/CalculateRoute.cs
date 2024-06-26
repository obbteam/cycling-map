using System.Net.Http;
using System.Windows.Documents;

namespace cycling_map;

public class CalculateRoute
{
    private static readonly HttpClient _client = new HttpClient();


    // Asynchronously get route data from TomTom Routing API
    public static async Task<Route> GetRouteAsync(List<Location> initialPoints, string apiKey, string travelMode)
    {
        try
        {
            // Convertion of list of Route Points to a string format location:location
            string locations = string.Join(":", initialPoints.Select(p => p.Format()));

            // Setup the endpoint URL with your API key and route coordinates
            string url =
                $"https://api.tomtom.com/routing/1/calculateRoute/{locations}/json?travelMode={travelMode}&key={apiKey}";

            // Make the asynchronous GET request
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Throw if not a success code.

            // Read the response as a string asynchronously
            string responseBody = await response.Content.ReadAsStringAsync();

            Route routeInfo = new Route();

            var RouteParse = new RouteParser();
            routeInfo = RouteParse.JsonParse<Route>(responseBody);

            Console.Write(responseBody);


            // Output the response body to console (or handle it as needed)
            return routeInfo;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return new Route();
        }
    }
}