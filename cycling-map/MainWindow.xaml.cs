using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace cycling_map;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
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

    private Location calculateXYZToLatLon(int x, int y, int z)
    {
        var lon = (x / Math.Pow(2, z)) * 360.0 - 180.0;

        var n = Math.PI - (2.0 * Math.PI * y) / Math.Pow(2, z);
        var lat = (180.0 / Math.PI) * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));

        return new Location(lat, lon);
    }

    private (int, int) calculateLonLatToXY(Location location)
    {
        double xyTilesCount = Math.Pow(2, _zoomLevel);
        int x = (int)Math.Floor((location.Lon() + 180.0) / 360.0 * xyTilesCount);
        int y = (int)Math.Floor(
            (1.0 - Math.Log(Math.Tan(location.Lat() * Math.PI / 180.0) +
                            1.0 / Math.Cos(location.Lat() * Math.PI / 180.0)) / Math.PI) / 2.0 * xyTilesCount);
        return (x, y);
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

            firstPoint = GeocodeParser.ParseGeocode(coordJson);
            secondPoint = GeocodeParser.ParseGeocode(coordJson2);
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

                    var renderBitmap = DrawRoute(bitmap, RoutePoints, firstPoint, secondPoint);
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


    private RenderTargetBitmap DrawRoute(BitmapImage tile, List<Location> RoutePoints, Location firstPoint,
        Location secondPoint)
    {
        DrawingVisual drawingVisual = new DrawingVisual();
        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawImage(tile, new Rect(0, 0, tile.PixelWidth, tile.PixelHeight));
            if (RoutePoints.Count > 0)
            {
                for (int i = 0; i + 1 < RoutePoints.Count; i++)
                {
                    DrawLine(drawingContext, this.RoutePoints[i], this.RoutePoints[i + 1], _zoomLevel, Colors.Aquamarine);
                }
            }

            if (this.firstPoint != null) DrawPoint(drawingContext, firstPoint, _zoomLevel, Colors.Blue);

            if (this.secondPoint != null) DrawPoint(drawingContext, secondPoint, _zoomLevel, Colors.Red);
        }

        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(tile.PixelWidth, tile.PixelHeight, tile.DpiX,
            tile.DpiY, PixelFormats.Pbgra32);
        renderBitmap.Render(drawingVisual);
        return renderBitmap;
    }

    private void DrawPoint(DrawingContext drawingContext, Location point, int zoom, Color color)
    {
        var (pixelX, pixelY) = LatLonToPixelXY(point.Lat(), point.Lon(), zoom);
        var (localX, localY) = (pixelX % 512, pixelY % 512);

        drawingContext.DrawEllipse(new SolidColorBrush(color), null, new System.Windows.Point(localX, localY), 3, 3);
    }

    private void DrawLine(DrawingContext drawingContext, Location point1, Location point2, int zoom, Color color)
    {
        var (pixelX1, pixelY1) = LatLonToPixelXY(point1.Lat(), point1.Lon(), zoom);
        var (localX1, localY1) = (pixelX1 % 512, pixelY1 % 512);
        var (pixelX2, pixelY2) = LatLonToPixelXY(point2.Lat(), point2.Lon(), zoom);
        var (localX2, localY2) = (pixelX2 % 512, pixelY2 % 512);

        drawingContext.DrawLine(new Pen(new SolidColorBrush(color), 2), new System.Windows.Point(localX1, localY1),
            null, new System.Windows.Point(localX2, localY2), null);
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


    private async void btnLoadMap_Click(object sender, RoutedEventArgs e)
    {
        address = Uri.EscapeDataString(txtAddress1.Text); // Corrected to get text from input fields
        address2 = Uri.EscapeDataString(txtAddress2.Text); // Corrected to get text from input fields

        RoutePoints.Clear();

        await GetGeoCode(new HttpClient());
        _zoomLevel = 22;
        calculateBoundingBox();
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
        var response = await CalculateRoute.GetRouteAsync(initialPoints, ApiKey, responseSummary);
        routeInfoLabel.Content = $"Distance: {responseSummary.LengthInMeters.ToString()} meters.\n Departure time:{responseSummary.DepartureTime.ToString()}.\n arrival time:{responseSummary.ArrivalTime.ToString()}.\n";
        RoutePoints.AddRange(response);
        _zoomLevel = 22;

        calculateBoundingBox();

        LoadMapTile(_zoomLevel, _tileX, _tileY);

        MessageBox.Show("Route calculation complete.");
    }

    private struct BoundingBox
    {
        public double left, right, top, bottom;

        public BoundingBox(double left, double right, double top, double bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }

    private void calculateBoundingBox()
    {
        var bbox = new BoundingBox(firstPoint.Lon(), firstPoint.Lon(), firstPoint.Lat(), firstPoint.Lat());
        foreach (var point in RoutePoints)
        {
            if (point.Lat() > bbox.top) bbox.top = point.Lat();
            if (point.Lat() < bbox.bottom) bbox.bottom = point.Lat();

            if (point.Lon() < bbox.left) bbox.left = point.Lon();

            if (point.Lon() > bbox.right) bbox.right = point.Lon();
        }

        if (secondPoint.Lat() > bbox.top) bbox.top = secondPoint.Lat();

        if (secondPoint.Lat() < bbox.bottom) bbox.bottom = secondPoint.Lat();

        if (secondPoint.Lon() < bbox.left) bbox.left = secondPoint.Lon();

        if (secondPoint.Lon() > bbox.right) bbox.right = secondPoint.Lon();


        double bboxWidthDeg = bbox.right - bbox.left;
        double bboxHeightDeg = bbox.top - bbox.bottom;

        double bboxWidthT = (1 - Math.Cos(bboxWidthDeg)) / 2 * 6371000;
        double bboxHeightT = (1 - Math.Cos(bboxHeightDeg)) / 2 * 6371000;

        double bboxSize = Math.Max(bboxWidthT, bboxHeightT);

        _zoomLevel = 22 - (int)Math.Log2(bboxSize);
        var topleft = new Location(bbox.top, bbox.left);
        var (tlx, tly) = calculateLonLatToXY(topleft);
        var botright = new Location(bbox.bottom, bbox.right);
        var (brx, bry) = calculateLonLatToXY(botright);
        while ((tlx != brx || tly != bry) && _zoomLevel > 0)
        {
            _zoomLevel--;
            (tlx, tly) = calculateLonLatToXY(topleft);
            (brx, bry) = calculateLonLatToXY(botright);
        }

        (_tileX, _tileY) = calculateLonLatToXY(botright);
    }
}