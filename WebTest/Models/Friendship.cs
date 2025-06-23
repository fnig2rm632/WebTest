namespace WebTest.Models;

public class Friendship
{
    public DateTime FriendshipDate { get; set; } = DateTime.UtcNow; 
    public string UserId { get; set; } 
    public string FriendId { get; set; } 
    public string Status { get; set; }
    public User User { get; set; }
    public User Friend { get; set; }
}