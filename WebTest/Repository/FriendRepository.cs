using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebTest.Data;
using WebTest.Dtos.Friend;
using WebTest.Interfaces;
using WebTest.Interfaces.Repository;
using WebTest.Migrations;
using WebTest.Models;

namespace WebTest.Repository;

public class FriendRepository(ApplicationDbContext context) : IFriendRepository
{
    public async Task<Friendship> GetFriendAsync(string userId , string friendId)
    {
        return (await context.Friendship.FirstOrDefaultAsync(x => x.UserId == userId || x.FriendId == friendId))!;
    }
    public async Task<List<Friendship>> GetFriendsListAsync(string userId)
    {
        return await context.Friendship
            .Where(x => x.UserId == userId || x.FriendId == userId)
            .ToListAsync();
    }
    public async Task<bool> SendFriendRequestAsync(string fromUserId, string toUserId)
    {
        if (await IsFriendRequestExistsAsync(fromUserId, toUserId) || 
            await IsFriendsAsync(fromUserId, toUserId))
        {
            return false;
        }

        var friendship = new Friendship
        {
            UserId = fromUserId,
            FriendId = toUserId,
            Status = "Pending",
            FriendshipDate = DateTime.UtcNow
        };

        context.Friendship.Add(friendship);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> AcceptFriendRequestAsync(string fromUserId, string toUserId)
    {
        var request = await context.Friendship.FirstOrDefaultAsync(x => (x.UserId == fromUserId && x.FriendId == toUserId) || ( x.UserId == toUserId && x.FriendId == fromUserId));
        
        if (request == null || request.Status != "Pending")
        {
            return false;
        }

        request.Status = "Accepted";
        request.FriendshipDate = DateTime.UtcNow;

        context.Friendship.Update(request);
        
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteFriendAsync(string userId, string friendId)
    {
        var request = await context.Friendship
            .FirstOrDefaultAsync(x => (x.UserId == userId && x.FriendId == friendId) 
                                      || (x.UserId == friendId && x.FriendId == userId));
        
        if (request == null)
        {
            return false;
        }
        
        context.Friendship.Remove(request);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> IsFriendsAsync(string userId1, string userId2)
    {
        return await context.Friendship
            .AnyAsync(x => (x.UserId == userId1 && x.FriendId == userId2 && x.Status == "Accepted") ||
                           (x.UserId == userId2 && x.FriendId == userId1 && x.Status == "Accepted"));
    }
    public async Task<bool> IsFriendRequestExistsAsync(string userId1, string userId2)
    {
        return await context.Friendship
            .AnyAsync(x => (x.UserId == userId1 && x.FriendId == userId2 && x.Status == "Pending") ||
                           (x.UserId == userId2 && x.FriendId == userId1 && x.Status == "Pending"));
    }
}