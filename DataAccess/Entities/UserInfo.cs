namespace DataAccess.Entities;
public class UserInfo
{
    public Name Name { get; init; } = new();
    public Location Location { get; init; } = new();
    public Dob Dob { get; init; } = new();
}
