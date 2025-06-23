using System.Net.WebSockets;
using WebTest.Models;

namespace WebTest.Interfaces.Manager;

public interface IWedSocketManager
{
    Task AddPlayer(Guid playerId, WebSocket webSocket);
    Task RemovePlayer(Guid playerId);
    Task SendGameState(Guid playerId, GameStatus gameStatus);
    Task<bool> TryMatchPlayers(Guid player1, Guid player2, Game game);
    Task<bool> WaitForPlayersToBeReady(Guid player1, Guid player2);
}