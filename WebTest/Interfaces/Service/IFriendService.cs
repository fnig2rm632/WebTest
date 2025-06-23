using WebTest.Dtos.Friend;
using WebTest.Models;

namespace WebTest.Interfaces.Service;

public interface IFriendService
{
    Task<ServiceResponse<List<FriendResponseDto>>> GetFriendsList(string userId);
    Task<ServiceResponse<List<FriendResponseDto>>> SearchFriends(string query, string userId);
    Task<ServiceResponse<bool>> SendFriendRequest(string fromUserId, string toUserId);
    Task<ServiceResponse<bool>> AcceptFriendRequest(string fromUserId, string toUserId);
    Task<ServiceResponse<bool>> DeleteFriend(string userId, string friendId);
}