using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cycling_map;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string ApiKey = "4Qz1nAMjo2oqVAGoe1VteCrgVlR6ieiS";
    private int _zoomLevel = 13;
    private int _tileX = 4252;
    private int _tileY = 2697;
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
    }

    private Location calculateBounds(int x, int y, int z)
    {
        var lon = (x / Math.Pow(2, z)) * 360.0 - 180.0;

        var n = Math.PI - (2.0 * Math.PI * y) / Math.Pow(2, z);
        var lat = (180.0 / Math.PI) * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));

        return new Location(lat, lon);
    }

    public async Task GetGeoCode(HttpClient client)
    {
        coordUrl =
            $"https://api.tomtom.com/search/2/geocode/{address}.json?storeResult=false&limit=1&countrySet=NL&view=Unified&key={ApiKey}";
        coordUrl2 =
            $"https://api.tomtom.com/search/2/geocode/{address2}.json?storeResult=false&limit=1&countrySet=NL&view=Unified&key={ApiKey}";


        var coordResponse = await client.GetAsync(coordUrl);
        var coordResponse2 = await client.GetAsync(coordUrl2);

        coordResponse.EnsureSuccessStatusCode();
        coordResponse2.EnsureSuccessStatusCode();


        var coordJson = await coordResponse.Content.ReadAsStringAsync();
        var coordJson2 = await coordResponse2.Content.ReadAsStringAsync();

        firstPoint = GeocodeParser.ParseGeocode(coordJson);
        secondPoint = GeocodeParser.ParseGeocode(coordJson2);
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

                if (SearchPressed)
                {
                    await GetGeoCode(client);
                }

                var imageData = await response.Content.ReadAsByteArrayAsync();
                using (var ms = new MemoryStream(imageData))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    var renderBitmap = DrawPointOnTile(bitmap, firstPoint, secondPoint);
                    if (RoutePoints.Count > 0) renderBitmap = DrawRoute(bitmap, RoutePoints, firstPoint, secondPoint);
                    mapImage.Source = renderBitmap;
                }

                if (firstPoint != null && secondPoint != null)
                {
                    Location topLeft = calculateBounds(x, y, zoom);
                    Location botRight = calculateBounds(x + 1, y + 1, zoom);
                    // Add code to display pixelCoordinate on the map, if needed.
                }
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


    private RenderTargetBitmap DrawPointOnTile(BitmapImage tile, Location firstPoint, Location secondPoint)
    {
        DrawingVisual drawingVisual = new DrawingVisual();
        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawImage(tile, new Rect(0, 0, tile.PixelWidth, tile.PixelHeight));

            if (firstPoint != null)
            {
                DrawPoint(drawingContext, firstPoint, tile.PixelWidth, tile.PixelHeight, _zoomLevel, Colors.Red);
            }

            if (secondPoint != null)
            {
                DrawPoint(drawingContext, secondPoint, tile.PixelWidth, tile.PixelHeight, _zoomLevel, Colors.Blue);
            }
        }

        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(tile.PixelWidth, tile.PixelHeight, tile.DpiX,
            tile.DpiY, PixelFormats.Pbgra32);
        renderBitmap.Render(drawingVisual);
        return renderBitmap;
    }


    private RenderTargetBitmap DrawRoute(BitmapImage tile, List<Location> RoutePoints, Location firstPoint,
        Location secondPoint)
    {
        DrawingVisual drawingVisual = new DrawingVisual();
        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawImage(tile, new Rect(0, 0, tile.PixelWidth, tile.PixelHeight));


            foreach (var point in RoutePoints)
            {
                DrawPoint(drawingContext, point, tile.PixelWidth, tile.PixelHeight, _zoomLevel, Colors.CornflowerBlue);
            }

            DrawPoint(drawingContext, firstPoint, tile.PixelWidth, tile.PixelHeight, _zoomLevel, Colors.Blue);

            DrawPoint(drawingContext, secondPoint, tile.PixelWidth, tile.PixelHeight, _zoomLevel, Colors.Red);
        }

        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(tile.PixelWidth, tile.PixelHeight, tile.DpiX,
            tile.DpiY, PixelFormats.Pbgra32);
        renderBitmap.Render(drawingVisual);
        return renderBitmap;
    }

    private void DrawPoint(DrawingContext drawingContext, Location point, int tileX, int tileY, int zoom, Color color)
    {
        var (pixelX, pixelY) = LatLonToPixelXY(point.Lat(), point.Lon(), zoom);
        var (localX, localY) = (pixelX % 512, pixelY % 512);

        drawingContext.DrawEllipse(new SolidColorBrush(color), null, new System.Windows.Point(localX, localY), 4, 4);
    }

    public static (int pixelX, int pixelY) LatLonToPixelXY(double lat, double lon, int zoom)
    {
        // Ensure latitude is within bounds
        lat = Math.Max(Math.Min(lat, 85.05112878), -85.05112878);

        // Convert latitude and longitude to pixel coordinates
        double sinLatitude = Math.Sin(lat * Math.PI / 180);
        int mapSize = 512 << zoom; // Size of the map in pixels at the given zoom level
        double pixelX = ((lon + 180) / 360) * mapSize;
        double pixelY = ((0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI)) * mapSize);

        // Determine the local pixel coordinates within the tile
        int localX = (int)pixelX % 512;
        int localY = (int)pixelY % 512;

        return (localX, localY);
    }


    private void btnLoadMap_Click(object sender, RoutedEventArgs e)
    {
        SearchPressed = true;
        address = txtAddress1.Text + " enschede";
        address2 = txtAddress2.Text + " enschede";
        address = Uri.EscapeDataString(address); // Corrected to get text from input fields
        address2 = Uri.EscapeDataString(address2); // Corrected to get text from input fields

        DisplayAddress.Content = $"From {address.ToString()} to {address2.ToString()}";
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

        List<Location> routePoints = new List<Location>
        {
            firstPoint,
            secondPoint
        };

        var response = await CalculateRoute.GetRouteAsync(routePoints, ApiKey);

        foreach (var point in response)
        {
            RoutePoints.Add(point);
        }

        LoadMapTile(_zoomLevel, _tileX, _tileY);

        MessageBox.Show("Route calculation complete.");
    }
}