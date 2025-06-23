using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebTest.Data;
using WebTest.Dtos.Game;
using WebTest.Interfaces.Manager;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Models;

namespace WebTest.Services;

public class WebSocketManager(
    IGameLogicService gameLogicService,
    ILogger<WebSocketManager> logger,
    IServiceProvider serviceProvider) : IWedSocketManager
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();
    private readonly object _locker = new();
    ConcurrentDictionary<Guid, TaskCompletionSource<bool>> _playerReady = new();
    
    public async Task AddPlayer(Guid playerId, WebSocket webSocket)
    {
        lock (_locker)
        {
            _sockets[playerId] = webSocket;

            _playerReady.AddOrUpdate(
                playerId,
                _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously),
                (_, existing) =>
                {
                    if (existing.Task.IsCompleted)
                    {
                        return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    }
                    return existing;
                });
        }

        logger.LogInformation("Player {PlayerId} added successfully", playerId);

        // Запускаем приём сообщений
        var receiveTask = WaitForMessages(playerId, webSocket);

        // Отмечаем игрока как готового
        if (_playerReady.TryGetValue(playerId, out var tcs))
        {
            tcs.TrySetResult(true);
        }

        // Попытка матчмейкинга
        await TryStartMatchmakingIfReady(playerId);

        await receiveTask;
    }

    private async Task TryStartMatchmakingIfReady(Guid playerId)
    {
        using var scope = serviceProvider.CreateScope();
        var gameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        // Здесь надо получить Game для playerId
        var game = await gameRepository.FindGameForIdPlayers(playerId.ToString());
        if (game == null) return;

        // Определяем соперника
        Guid? opponentId = null;
        if (game.PlayerWhiteId == playerId.ToString() && game.PlayerBlackId != "e1856c00-222b-4857-b1ce-c303945b9967")
        {
            opponentId = Guid.Parse(game.PlayerBlackId);
        }
        else if (game.PlayerBlackId == playerId.ToString())
        {
            opponentId = Guid.Parse(game.PlayerWhiteId);
        }

        if (opponentId == null) return;

        // Проверяем, готов ли соперник
        if (_playerReady.TryGetValue(opponentId.Value, out var opponentTcs) && opponentTcs.Task.IsCompleted)
        {
            // Оба готовы — вызываем матчмейкинг
            bool success = await TryMatchPlayers(playerId, opponentId.Value, game);
            if (!success)
            {
                logger.LogWarning("Matchmaking failed for players {Player1} and {Player2}", playerId, opponentId);
            }
        }
        else
        {
            logger.LogInformation("Opponent {OpponentId} not ready yet for player {PlayerId}", opponentId, playerId);
        }
    }

    private async Task CloseSocketAsync(WebSocket socket)
    {
        try
        {
            if (socket?.State == WebSocketState.Open)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Replaced by new connection",
                    CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error closing socket");
        }
    }

    public Task RemovePlayer(Guid playerId)
    {
        logger.LogInformation("Player {PlayerId} removed successfully", playerId);
        try
        {
            _sockets.TryRemove(playerId, out _);
            _playerReady.TryRemove(playerId, out _);
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error removing player");
            return Task.CompletedTask;
        }
    }

    public async Task SendGameState(Guid playerId, GameStatus gameStatus)
    {
        Console.WriteLine($"SendGameState -> {playerId}");
        if (_sockets.TryGetValue(playerId, out var socket))
        {
            Console.WriteLine($"Socket found for {playerId}, state: {socket.State}");
            try
            {
                if (socket.State == WebSocketState.Open)
                {
                    var json = JsonSerializer.Serialize(gameStatus);
                    var buffer = Encoding.UTF8.GetBytes(json);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    Console.WriteLine($"Socket for {playerId} is not open.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка отправки сокету игрока {playerId}: {e.GetType().Name} — {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Socket not found for {playerId}");
        }
    }
    
    public async Task<bool> WaitForPlayersToBeReady(Guid player1, Guid player2)
    {
        var tcs1 = _playerReady.GetOrAdd(player1, _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));
        var tcs2 = _playerReady.GetOrAdd(player2, _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));

        var timeoutTask = Task.Delay(3000);
        var readyTask = Task.WhenAll(tcs1.Task, tcs2.Task);

        var completed = await Task.WhenAny(readyTask, timeoutTask);
        return completed == readyTask;
    }

    public async Task<bool> TryMatchPlayers(Guid player1, Guid player2, Game game)
    {
        logger.LogInformation("Attempting to match {Player1} and {Player2}", player1, player2);

        /*if (!await WaitForPlayersToBeReady(player1, player2))
        {
            logger.LogError("Players not ready after timeout.");
            return false;
        }*/

        // Проверяем сокеты и их состояние
        if (!_sockets.TryGetValue(player1, out var socket1) || socket1.State != WebSocketState.Open ||
            !_sockets.TryGetValue(player2, out var socket2) || socket2.State != WebSocketState.Open)
        {
            logger.LogError("One or both sockets are missing or closed. player1: {Player1State}",
                socket1?.State.ToString() ?? "null");
            return false;
        }

        try
        {
            var initialGameState = await gameLogicService.InitializeGame(player1, player2, game);

            await Task.WhenAll(
                SendGameState(player1, initialGameState),
                SendGameState(player2, initialGameState));

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during matchmaking");
            return false;
        }
    }

    private async Task WaitForMessages(Guid playerId, WebSocket webSocket)
{
    GameStatus updatedGameState = new GameStatus();
    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var messageBytes = new List<byte>();
            WebSocketReceiveResult result;
            do
            {
                var buffer = new byte[1024 * 16];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                messageBytes.AddRange(buffer.Take(result.Count));
            }
            while (!result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                logger.LogInformation("Player {PlayerId} closed connection", playerId);
                break;
            }

            var message = Encoding.UTF8.GetString(messageBytes.ToArray());
            logger.LogDebug("Received from {PlayerId}: {Message}", playerId, message);

            var gameMove = JsonSerializer.Deserialize<GameMove>(message);
            updatedGameState = await gameLogicService.ProcessMove(playerId, gameMove);

            await Task.WhenAll(
                SendGameState(updatedGameState.Player1Id, updatedGameState),
                SendGameState(updatedGameState.Player2Id, updatedGameState));
        }
    }
    catch (WebSocketException ex)
    {
        logger.LogWarning(ex, "WebSocket error for {PlayerId}", playerId);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error for {PlayerId}", playerId);
    }
    finally
    {
        using var scope = serviceProvider.CreateScope();
        var gameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        
        var gameStatus = await gameRepository.FindCreateGameForSearch(playerId.ToString());
        if (gameStatus != null)
        {
            var opponentId = (gameStatus.PlayerWhiteId == playerId.ToString())
                ? gameStatus.PlayerBlackId
                : gameStatus.PlayerWhiteId;

            gameStatus.WinnerId = opponentId;
            gameStatus.EndTime = DateTime.UtcNow;

            updatedGameState.WinPlayer = Guid.Parse(opponentId);
            await Task.WhenAll(
                SendGameState(playerId, updatedGameState),
                SendGameState(Guid.Parse(opponentId), updatedGameState));

            _ = await gameRepository.UpdateGame(gameStatus);
        }

        await RemovePlayer(playerId);
    }
}
}