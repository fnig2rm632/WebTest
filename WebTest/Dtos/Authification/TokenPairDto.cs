namespace WebTest.Dtos.Authification;

public class TokenPairDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}