using WebTest.Dtos.Game;
using WebTest.Dtos.User;

namespace WebTest.Interfaces.Service;

public interface IMatchmakingService
{
    Task<UserResponseDto> StartFindMatchmakingAsync(GameResponseDto game);

    Task<UserResponseDto> FindMatchmakingAsync(GameResponseDto game);

    Task<bool> CancelMatchmakingAsync(GameResponseDto game);

    Task<UserResponseDto> StartMatchmakingAsync(GameResponseDto gameDto);
}