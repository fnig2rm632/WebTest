using WebTest.Models;

namespace WebTest.Interfaces.Repository;

public interface IUserRepository
{
    // Основные методы
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> CreateAsync(User user, string password);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(string id);
    Task<List<User>> SearchUsersAsync(string query);
    
    // Методы для работы с Refresh Token
    Task<bool> SetRefreshTokenAsync(string userId, string? refreshToken, DateTime? expiryTime);
    Task<(string? RefreshToken, DateTime? ExpiryTime)> GetRefreshTokenAsync(string userId);
    
    // Вспомогательные методы
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<bool> UserExistsAsync(string username);
}