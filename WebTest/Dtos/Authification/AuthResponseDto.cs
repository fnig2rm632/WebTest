namespace WebTest.Dtos.Authification;

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
}