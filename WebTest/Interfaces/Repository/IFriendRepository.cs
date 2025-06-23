using WebTest.Models;

namespace WebTest.Interfaces.Repository;

public interface IFriendRepository
{
    // Основные методы 
    Task<Friendship> GetFriendAsync(string userId, string friendId);
    Task<List<Friendship>> GetFriendsListAsync(string userId);
    Task<bool> SendFriendRequestAsync(string fromUserId, string toUserId);
    Task<bool> AcceptFriendRequestAsync(string fromUserId, string toUserId);
    Task<bool> DeleteFriendAsync(string userId, string friendId);
        
    // Вспомогательные методы
    Task<bool> IsFriendsAsync(string userId1, string userId2);
    Task<bool> IsFriendRequestExistsAsync(string fromUserId, string toUserId);
}