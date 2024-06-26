using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cycling_map;

public partial class MainWindow : Window
{
    private const string ApiKey = "4Qz1nAMjo2oqVAGoe1VteCrgVlR6ieiS";
    private int _zoomLevel = 13;

    private int _tileX = 4250;

    //4252,4250
    private int _tileY = 2696;

    //2697,2696
    private const double tileSize = 9.55463;
    string address = "Molenstraat 74";
    string address2 = "Oosterstraat 190";
    private string coordUrl;
    private string coordUrl2;
    private Location firstPoint;
    private Location secondPoint;
    private List<Location> RoutePoints = new List<Location>();
    private bool SearchPressed = false;


    public MainWindow()
    {
        InitializeComponent();
        LoadMapTile(_zoomLevel, _tileX, _tileY); // load initial
        //TODO: zoom out if the route is too long
        //ASDSDJSDLKSDJ
    }

    public async Task GetGeoCode(HttpClient client)
    {
        try
        {
            coordUrl =
                $"https://api.tomtom.com/search/2/geocode/{address}.json?storeResult=false&limit=1&view=Unified&key={ApiKey}";
            coordUrl2 =
                $"https://api.tomtom.com/search/2/geocode/{address2}.json?storeResult=false&limit=1&view=Unified&key={ApiKey}";


            var coordResponse = await client.GetAsync(coordUrl);
            var coordResponse2 = await client.GetAsync(coordUrl2);

            coordResponse.EnsureSuccessStatusCode();
            coordResponse2.EnsureSuccessStatusCode();


            var coordJson = await coordResponse.Content.ReadAsStringAsync();
            var coordJson2 = await coordResponse2.Content.ReadAsStringAsync();

            var GeoParse = new GeocodeParser();

            firstPoint = GeoParse.JsonParse<Location>(coordJson);
            secondPoint = GeoParse.JsonParse<Location>(coordJson2);
        }
        catch (HttpRequestException e)
        {
            MessageBox.Show($"Request error: {e.Message}");
        }
    }

    private async void LoadMapTile(int zoom, int x, int y)
    {
        string url =
            $"https://api.tomtom.com/map/1/tile/basic/main/{zoom}/{x}/{y}.png?tileSize=512&view=Unified&language=NGT&key={ApiKey}";
        using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
        {
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();


                var imageData = await response.Content.ReadAsByteArrayAsync();
                using (var ms = new MemoryStream(imageData))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    var renderBitmap = PointsComputation.DrawRoute(bitmap, RoutePoints, firstPoint, secondPoint, zoom);
                    mapImage.Source = renderBitmap;
                }

                //if (firstPoint != null && secondPoint != null)
                //{
                //  Location topLeft = calculateXYZToLatLon(x, y, zoom);
                // Location botRight = calculateXYZToLatLon(x + 1, y + 1, zoom);
                // Add code to display pixelCoordinate on the map, if needed.
                //}
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show($"Request error: {e.Message}");
            }
            catch (TaskCanceledException e)
            {
                MessageBox.Show($"Request timeout: {e.Message}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"General error: {e.Message}");
            }
        }
    }



    private async void btnLoadMap_Click(object sender, RoutedEventArgs e)
    {
        address = Uri.EscapeDataString(txtAddress1.Text); // Corrected to get text from input fields
        address2 = Uri.EscapeDataString(txtAddress2.Text); // Corrected to get text from input fields

        RoutePoints.Clear();

        await GetGeoCode(new HttpClient());
        //calculateBoundingBox();
        (_tileX,_tileY,_zoomLevel) = PointsComputation.calculateBoundingBox(firstPoint, secondPoint, RoutePoints);
        LoadMapTile(_zoomLevel, _tileX, _tileY);
    }

    private void txtAddress_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            address = txtAddress1.Text;
            e.Handled = true; // Prevents further handling of the Enter key event
        }
    }

    private void txtAddress2_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            address2 = txtAddress2.Text;
            e.Handled = true; // Prevents further handling of the Enter key event
        }
    }

    private async void btnCalculate_Click(object sender, RoutedEventArgs e)
    {
        if (firstPoint == null || secondPoint == null)
        {
            MessageBox.Show("Please search for addresses first.");
            return;
        }

        List<Location> initialPoints = new List<Location>
        {
            firstPoint,
            secondPoint
        };
        var responseSummary = new Summary();
        var response = await CalculateRoute.GetRouteAsync(initialPoints, ApiKey);

        collectSummaryInfo(response);

        RoutePoints = PointsComputation.collectRoutePoints(response);
        _zoomLevel = 22;
        (_tileX, _tileY, _zoomLevel) = PointsComputation.calculateBoundingBox(firstPoint, secondPoint, RoutePoints);
        LoadMapTile(_zoomLevel, _tileX, _tileY);

        MessageBox.Show("Route calculation complete.");
    }

    void collectSummaryInfo(Route RouteInfo)
    {
        routeInfoLabel.Content = $"Distance: {RouteInfo.Summary.LengthInMeters.ToString()} meters.\n " +
                                 $"Departure time:{RouteInfo.Summary.DepartureTime.ToString()}.\n " +
                                 $"arrival time:{RouteInfo.Summary.ArrivalTime.ToString()}.\n";
    }
}