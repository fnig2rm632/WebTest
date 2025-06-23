using WebTest.Dtos.Authification;
using WebTest.Models;

namespace WebTest.Mappers;

public class AuthMapper
{
    public static AuthResponseDto ToAuthResponseDto(TokenPair tokens, User user)
    {
        return new AuthResponseDto
        {
            Success = true,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            UserId = user.Id,
            Username = user.UserName
        };
    }

    public static AuthResponseDto ToAuthResponseDto(string error)
    {
        return new AuthResponseDto
        {
            Success = false,
            Error = error
        };
    }

    public static TokenPairDto ToTokenPairDto(TokenPair tokens)
    {
        return new TokenPairDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }
}