using System.ComponentModel.DataAnnotations;

namespace WebTest.Dtos.Authification;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;
}