using Microsoft.AspNetCore.Identity;
using WebTest.Dtos.Friend;
using WebTest.Dtos.User;
using WebTest.Interfaces;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Mappers;

namespace WebTest.Services;

public class UserService(
    IUserRepository userRepository,
    IGameRepository gameRepository,
    IFriendRepository friendRepository) : IUserService
{
    // Взять пользователя
    public async Task<UserResponseDto> GetUser(string userId)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                return UserMappers.ToUserResponseDto("User not found");
            }
            
            return user!.ToUserResponseDto();
        }
        catch (Exception e)
        {
            return UserMappers.ToUserResponseDto(e.Message);
        }
    }

    // Взять профиль пользователя
    public async Task<UserResponseDto> GetUserProfile(string userId)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                return UserMappers.ToUserResponseDto("User not found");
            }
            
            var games = GameMapper.ToGameResponseListDto(await gameRepository.GetGamesByIdAsync(userId));;

            foreach (var game in games)
            {
                game.PlayerBlackName = userRepository.GetByIdAsync(game.PlayerBlackId).Result!.UserName;
                game.PlayerWhiteName = userRepository.GetByIdAsync(game.PlayerWhiteId).Result!.UserName;
            }
            
            var friendships = await friendRepository.GetFriendsListAsync(userId);

            List<FriendResponseDto> responseDtos = new();
            
            foreach (var friendship in friendships)
            {
                var responseUser = await userRepository.GetByIdAsync(friendship.FriendId);
                responseDtos.Add(FriendshipMapper.ToFullFriendResponseDto(friendship, responseUser!));
            }
            
            return UserMappers.ToUserResponseDto(user!,games,responseDtos);
        }
        catch (Exception e)
        {
            return UserMappers.ToUserResponseDto(e.Message);
        }
    }
}