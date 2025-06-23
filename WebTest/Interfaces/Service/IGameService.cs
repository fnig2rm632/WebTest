using WebTest.Dtos.Friend;
using WebTest.Dtos.Game;
using WebTest.Models;

namespace WebTest.Interfaces.Service;

public abstract class IGameService
{
    public abstract Task<ServiceResponse<List<GameResponseDto>>> GetHistoryList(string userId);
    public abstract Task<GameResponseDto?> FindGameForDataAsync(GameResponseDto responseDto);
    public abstract Task<bool> UpdateGame(GameResponseDto responseDto);
}