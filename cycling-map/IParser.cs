namespace cycling_map;

public interface IParser
{
    T JsonParse<T>(string json);
}