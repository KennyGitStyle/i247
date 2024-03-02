namespace DataAccess.Entities;
public class Location
{
    public string Country { get; init; } = string.Empty;
    public Coordinates Coordinates { get; init; } = new();
}
