namespace cycling_map;

public class GeoJson
{
    public class Summary
    {
        public string query { get; set; }
        public string queryType { get; set; }
        public int queryTime { get; set; }
        public int numResults { get; set; }
        public int offset { get; set; }
        public int totalResults { get; set; }
        public int fuzzyLevel { get; set; }
    }

    public class MatchConfidence
    {
        public double score { get; set; }
    }

    public class Address
    {
        public string streetNumber { get; set; }
        public string streetName { get; set; }
        public string municipality { get; set; }
        public string neighbourhood { get; set; }
        public string countrySubdivision { get; set; }
        public string countrySubdivisionName { get; set; }
        public string countrySubdivisionCode { get; set; }
        public string postalCode { get; set; }
        public string extendedPostalCode { get; set; }
        public string countryCode { get; set; }
        public string country { get; set; }
        public string countryCodeISO3 { get; set; }
        public string freeformAddress { get; set; }
        public string localName { get; set; }
    }

    public class Position
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class Viewport
    {
        public TopLeftPoint topLeftPoint { get; set; }
        public BtmRightPoint btmRightPoint { get; set; }
    }

    public class TopLeftPoint
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class BtmRightPoint
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class EntryPoint
    {
        public string type { get; set; }
        public Position position { get; set; }
    }

    public class Result
    {
        public string type { get; set; }
        public string id { get; set; }
        public double score { get; set; }
        public MatchConfidence matchConfidence { get; set; }
        public Address address { get; set; }
        public Position position { get; set; }
        public Viewport viewport { get; set; }
        public List<EntryPoint> entryPoints { get; set; }
    }

    public class GeocodeResponse
    {
        public Summary summary { get; set; }
        public List<Result> results { get; set; }
    }
}