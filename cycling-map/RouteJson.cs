using System.Text.Json.Serialization;

namespace cycling_map;

public class RouteJson
{
    [JsonPropertyName("formatVersion")]
    public string FormatVersion { get; set; }

    [JsonPropertyName("routes")]
    public List<Route> Routes { get; set; }
}

public class Route
{
    [JsonPropertyName("summary")]
    public Summary Summary { get; set; }

    [JsonPropertyName("legs")]
    public List<Leg> Legs { get; set; }

    [JsonPropertyName("sections")]
    public List<Section> Sections { get; set; }
}

public class Summary
{
    [JsonPropertyName("lengthInMeters")]
    public int LengthInMeters { get; set; }

    [JsonPropertyName("travelTimeInSeconds")]
    public int TravelTimeInSeconds { get; set; }

    [JsonPropertyName("trafficDelayInSeconds")]
    public int TrafficDelayInSeconds { get; set; }

    [JsonPropertyName("trafficLengthInMeters")]
    public int TrafficLengthInMeters { get; set; }

    [JsonPropertyName("departureTime")]
    public DateTime DepartureTime { get; set; }

    [JsonPropertyName("arrivalTime")]
    public DateTime ArrivalTime { get; set; }
}

public class Leg
{
    [JsonPropertyName("summary")]
    public Summary Summary { get; set; }

    [JsonPropertyName("points")]
    public List<Point> Points { get; set; }
}

public class Point
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

public class Section
{
    [JsonPropertyName("startPointIndex")]
    public int StartPointIndex { get; set; }

    [JsonPropertyName("endPointIndex")]
    public int EndPointIndex { get; set; }

    [JsonPropertyName("sectionType")]
    public string SectionType { get; set; }

    [JsonPropertyName("travelMode")]
    public string TravelMode { get; set; }
}