using WebTest.Dtos.User;

namespace WebTest.Interfaces.Service;

public interface IUserService
{
    Task<UserResponseDto> GetUser(string userId);
    Task<UserResponseDto> GetUserProfile(string userId);
}