using BusinessLogic.Dto;

namespace BusinessLogic.Service;

public interface IRandomUserService
{
    Task<IEnumerable<UserDto>> GetRandomUsersAsync(int numberOfUsers = 5);
}
