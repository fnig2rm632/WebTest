using System.ComponentModel.DataAnnotations;

namespace WebTest.Dtos.Authification;

public class RegisterRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 7)]
    public string Password { get; set; } = null!;

    [Required]
    public string Username { get; set; } = null!;
}