using WebTest.Dtos.Friend;
using WebTest.Dtos.User;
using WebTest.Interfaces;
using WebTest.Models;

namespace WebTest.Mappers;

public static class FriendshipMapper
{
    private static FriendResponseDto ToFriendResponseDto(Friendship friendship)
    {
        return new FriendResponseDto()
        {
            FriendId = friendship.FriendId,
            Status = friendship.Status
        };
    }
    
    public static FriendResponseDto ToFullFriendResponseDto(Friendship friendship , User user, string userId = null)
    {
        return new FriendResponseDto()
        {
            FriendId = user.Id,
            Status = friendship.Status,
            FriendName = user.UserName!
        };
    }
    
    public static FriendResponseDto ToFullNoFriendResponseDto(User user)
    {
        return new FriendResponseDto()
        {
            FriendId = user.Id,
            Status = "NotFriends",
            FriendName = user.UserName!
        };
    }
    
    public static List<FriendResponseDto> ToAuthResponseListDto(List<Friendship> source)
    {
        return source.Select(item => ToFriendResponseDto(item)).ToList();
    }
}