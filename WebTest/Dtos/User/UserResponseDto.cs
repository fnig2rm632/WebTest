using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Migrations;
using WebTest.Dtos.Friend;
using WebTest.Dtos.Game;
using WebTest.Models;

namespace WebTest.Dtos.User;

public class UserResponseDto
{
    public string Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<FriendResponseDto> Friends { get; set; } = new(); 
    public List<GameResponseDto> Games { get; set; } = new(); 
    public bool Success { get; set; }
    public string? Error { get; set; }
}