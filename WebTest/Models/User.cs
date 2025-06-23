using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebTest.Models;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiry { get; set; }

    // Связи
    public List<Friendship> Friends { get; set; } = new(); 
    public List<Game> GamesWiners { get; set; } 
    public List<Game> GamesAsWhite { get; set; } 
    public List<Game> GamesAsBlack { get; set; } 
}