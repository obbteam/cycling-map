using System.Globalization;

namespace cycling_map;

public class Location
{
    private double Latitude { get; }
    private double Longitude { get; }

    private string LatitudeString { get; }
    private string LongitudeString { get; }

    public double Lat() => Latitude;

    public double Lon() => Longitude;

    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        LatitudeString = Latitude.ToString(CultureInfo.InvariantCulture);
        Longitude = longitude;
        LongitudeString = Longitude.ToString(CultureInfo.InvariantCulture);
    }

    public string Format()
    {
        return $"{LatitudeString},{LongitudeString}";
    }
}