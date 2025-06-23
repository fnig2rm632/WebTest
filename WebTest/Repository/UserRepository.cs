using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebTest.Data;
using WebTest.Interfaces;
using WebTest.Interfaces.Repository;
using WebTest.Models;

namespace WebTest.Repository;

public class UserRepository(ApplicationDbContext context,UserManager<User> userManager) : IUserRepository
{
    // Основные методы
    public async Task<User?> GetByIdAsync(string id)
    {
        return await userManager.FindByIdAsync(id);
    }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await userManager.FindByNameAsync(username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<bool> CreateAsync(User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded;
    }
    
    public async Task<List<User>> SearchUsersAsync(string query)
    {
        return await context.Users
            .Where(x => x.UserName!.Contains(query))
            .Take(10)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user == null)
        {
            return false;
        }
        
        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }
    
    // Методы для работы с Refresh Token
    public async Task<bool> SetRefreshTokenAsync(string userId, string? refreshToken, DateTime? expiryTime)
    {
        var user = await userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return false;
        }
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = expiryTime;

        return await UpdateAsync(user);
    }

    public async Task<(string? RefreshToken, DateTime? ExpiryTime)> GetRefreshTokenAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return (user?.RefreshToken, user?.RefreshTokenExpiry);
    }
    
    // Вспомогательные методы
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await userManager.FindByNameAsync(username) != null;
    }
}