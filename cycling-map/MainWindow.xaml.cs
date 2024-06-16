using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    string address; //= "Molenstraat 74";
    string address2; // = "Oosterstraat 190";


    public MainWindow()
    {
        InitializeComponent();
        LoadMapTile(_zoomLevel, _tileX, _tileY); // load initial
    }

    private Point calculateBounds(int x, int y, int z)
    {
        var lon = (x / Math.Pow(2, z)) * 360.0 - 180.0;

        var n = Math.PI - (2.0 * Math.PI * y) / Math.Pow(2, z);
        var lat = (180.0 / Math.PI) * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));

        return new Point(lat, lon);
    }

    private async void LoadMapTile(int zoom, int x, int y)
    {
        string url =
            $"https://api.tomtom.com/map/1/tile/basic/main/{zoom}/{x}/{y}.png?tileSize=512&view=Unified&language=NGT&key={ApiKey}";
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(url);
                //var coordResponse = await client.GetAsync(coordUrl);
                //var coordResponse2 = await client.GetAsync(coordUrl2);

                response.EnsureSuccessStatusCode();
                //coordResponse.EnsureSuccessStatusCode();
                //coordResponse2.EnsureSuccessStatusCode();
                var imageData = await response.Content.ReadAsByteArrayAsync();

                //Point firstPoint = parseCoordinates(await coordResponse.Content.ReadFromJsonAsync<jsonContent[]>());
                //Point secondPoint = parseCoordinates(await coordResponse2.Content.ReadFromJsonAsync<jsonContent[]>());

                //Point TopLeft = calculateBounds(x, y, zoom);
                //Point BotRight = calculateBounds(x+1, y+1, zoom);

                //Point PixelCoordinate = new Point(mymap(firstPoint.X, TopLeft.X, BotRight.X), mymap(firstPoint.Y, TopLeft.Y, BotRight.Y));

                using (var ms = new MemoryStream(imageData))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    mapImage.Source = bitmap;
                }
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show($"Request error: {e.Message}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"General error: {e.Message}");
            }
        }
    }

    private double mymap(double x, double xl, double xr)
    {
        return (x - xl) / (xr - xl) * 512;
    }

    private Point parseCoordinates(jsonContent[] toParse)
    {
        double latitude = double.Parse(toParse[0].lat);
        double longtude = double.Parse(toParse[0].lon);
        return new Point(latitude, longtude);
    }

    private void btnLoadMap_Click(object sender, RoutedEventArgs e)
    {
        string coordUrl = $"https://nominatim.openstreetmap.org/search?format=jsonv2&q={address}";
        string coordUrl2 = $"https://nominatim.openstreetmap.org/search?format=json&q={address2}";
        DisplayAddress.Content = $"From {address.ToString()} to {address2.ToString()}";
    }

    private void txtAddress_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // Perform your action or update logic here
            address = txtAddress1.Text; // Prevents further handling of the Enter key event
        }
    }

    private void txtAddress2_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // Perform your action or update logic here
            address2 = txtAddress2.Text; // Prevents further handling of the Enter key event
        }
    }
}

public class jsonContent
{
    public string lat;
    public string lon;
}