using System.Security.Claims;
using WebTest.Models;

namespace WebTest.Interfaces.Service;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateRefreshToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}