using System.Security.Claims;
using WebTest.Dtos.Authification;
using WebTest.Interfaces;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Mappers;
using WebTest.Models;

namespace WebTest.Services;

public class AuthService(ITokenService tokenService, IUserRepository userRepository) : IAuthService
{
    // Метод регистрации пользователя
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var user = new User { UserName = request.Username, Email = request.Email };
        var created = await userRepository.CreateAsync(user, request.Password);

        if (!created)
        {
            return AuthMapper.ToAuthResponseDto("User creation failed");
        }

        var tokens = await GenerateTokens(user);
        return AuthMapper.ToAuthResponseDto(tokens, user);
    }

    // Вход в аккаунт
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
        {
            return AuthMapper.ToAuthResponseDto("User not found");
        }

        if (!await userRepository.CheckPasswordAsync(user,request.Password))
        {
            return AuthMapper.ToAuthResponseDto("Invalid password");
        }
        
        var tokens = await GenerateTokens(user);
        return AuthMapper.ToAuthResponseDto(tokens, user);
    }

    // Обновление Access Token
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = tokenService.ValidateToken(request.RefreshToken);

        if (principal == null)
        {
            return AuthMapper.ToAuthResponseDto("Invalid refresh token");
        }
        
        var userId = principal.Claims.FirstOrDefault()?.Value;
        var user = await userRepository.GetByIdAsync(userId!);

        if (user == null)
        {
            return AuthMapper.ToAuthResponseDto("User not found");
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return AuthMapper.ToAuthResponseDto("Expired refresh token time is over");
        }
        
        var tokens = await GenerateTokens(user);
        return AuthMapper.ToAuthResponseDto(tokens, user);
    }

    // Удаление Refresh Token
    public async Task RevokeTokenAsync(string userId)
    {
        await userRepository.SetRefreshTokenAsync(userId, null, null);
    }
    
    // Создание и добавление ключей
    private async Task<TokenPair> GenerateTokens(User user)
    {
        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = tokenService.CreateRefreshToken(user);

        await userRepository.SetRefreshTokenAsync(
            user.Id, 
            refreshToken, 
            DateTime.UtcNow.AddDays(7));

        return new TokenPair(accessToken, refreshToken);
    }
}