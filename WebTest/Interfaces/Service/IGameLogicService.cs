using WebTest.Models;

namespace WebTest.Interfaces.Service;

public interface IGameLogicService
{
    Task<GameStatus> InitializeGame(Guid player1, Guid player2, Game game);
    Task<GameStatus> ProcessMove(Guid playerId, GameMove move);
}