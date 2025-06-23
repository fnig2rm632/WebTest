using WebTest.Dtos.Friend;
using WebTest.Dtos.Game;
using WebTest.Dtos.User;
using WebTest.Models;

namespace WebTest.Mappers;

public static class UserMappers
{
    public static UserResponseDto ToUserResponseDto(this User userModel)
    {
        return new UserResponseDto()
        {
            Success = true,  
            Id = userModel.Id,
            Username = userModel.UserName,
            Email = userModel.Email,
            Games = null,    
            Friends = null
        };
    }
    
    public static UserResponseDto ToUserResponseDto(
        this User userModel, 
        List<GameResponseDto> games, 
        List<FriendResponseDto> friends)
    {
        return new UserResponseDto()
        {
            Success = true, 
            Id = userModel.Id,
            Username = userModel.UserName,
            Email = userModel.Email,
            Games = games ?? new List<GameResponseDto>(),  
            Friends = friends ?? new List<FriendResponseDto>()
        };
    }
    
    public static UserResponseDto ToUserResponseDto(string error)
    {
        return new UserResponseDto
        {
            Success = false,
            Error = error,
            Id = null,     
            Username = null,
            Email = null,
            Games = null,
            Friends = null
        };
    }
    
    public static UserResponseDto ToUserResponseTrueDto(string error)
    {
        return new UserResponseDto
        {
            Success = true,
            Error = error,
            Id = null,     
            Username = null,
            Email = null,
            Games = null,
            Friends = null
        };
    }
    
}