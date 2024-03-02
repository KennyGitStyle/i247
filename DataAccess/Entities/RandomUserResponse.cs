namespace DataAccess.Entities;
public class RandomUserResponse
{
    public IEnumerable<UserInfo> Results { get; init; } = Enumerable.Empty<UserInfo>();
}
