namespace cycling_map;

public class MathCalculations
{
    public static Location calculateXYZToLatLon(int x, int y, int z)
    {
        var lon = (x / Math.Pow(2, z)) * 360.0 - 180.0;

        var n = Math.PI - (2.0 * Math.PI * y) / Math.Pow(2, z);
        var lat = (180.0 / Math.PI) * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));

        return new Location(lat, lon);
    }
    
    public static (int, int) calculateLonLatToXY(Location location, int zoomLevel)
    {
        double xyTilesCount = Math.Pow(2, zoomLevel);
        int x = (int)Math.Floor((location.Lon() + 180.0) / 360.0 * xyTilesCount);
        int y = (int)Math.Floor(
            (1.0 - Math.Log(Math.Tan(location.Lat() * Math.PI / 180.0) +
                            1.0 / Math.Cos(location.Lat() * Math.PI / 180.0)) / Math.PI) / 2.0 * xyTilesCount);
        return (x, y);
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


}