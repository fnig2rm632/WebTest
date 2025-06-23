using System.Collections.Concurrent;
using WebTest.Dtos.Game;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Models;

namespace WebTest.Services;

public class GameLogicService : IGameLogicService
{
    private readonly ConcurrentDictionary<int, GameStatus> _activeGames = new();
    private readonly IServiceProvider _serviceProvider;

    public GameLogicService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<GameStatus> InitializeGame(Guid player1, Guid player2, Game game)
    {
        Console.WriteLine($"GameLogicService -> InitializeGame {game.Id}");
        var gameStatus = new GameStatus
        {
            GameId = game.Id,
            Player1Id = player1,
            Player2Id = player2,
            FieldMatrix = Enumerable.Range(0, game.BoardSize)
                .Select(_ => Enumerable.Repeat('\0', game.BoardSize).ToList())
                .ToList(),
            MoveHistory = new List<GameMove>(),
            ActivePlayerId = player1,
            CurrentMove = 1
        };
        
        _activeGames.TryAdd(gameStatus.GameId, gameStatus);
        return Task.FromResult(gameStatus);
    }

    public async Task<GameStatus> ProcessMove(Guid playerId, GameMove move)
    {
        Console.WriteLine($"ProcessMove --> {playerId} in {move.IdGame}");
        
        if (!_activeGames.TryGetValue(move.IdGame, out var gameStatus))
        {
            throw new KeyNotFoundException("Game not found");
        }

        try
        {
            // Проверяем не сдался ли противник
            if (move.Type == GameMove.MoveType.Resign)
            {
                Console.WriteLine($"Player {playerId} resigns");
                
                Console.WriteLine($"Player1 {gameStatus.Player1Id} Player2 {gameStatus.Player2Id}");
                using var scope = _serviceProvider.CreateScope();
                var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
                
                var winnerId = gameStatus.Player1Id == playerId 
                    ? gameStatus.Player2Id 
                    : gameStatus.Player1Id;
                
                gameStatus.WinPlayer = winnerId;
                
                bool answer = await gameService.UpdateGame(new GameResponseDto()
                {
                    Id = gameStatus.GameId,
                    PlayerWhiteId = gameStatus.Player1Id.ToString(),
                    PlayerBlackId = gameStatus.Player2Id.ToString(),
                    PlayerWinId = winnerId.ToString(),
                });

                Console.WriteLine($"Player {winnerId} resigns {answer}");
                
                _activeGames.TryRemove(move.IdGame, out _);
                return gameStatus;
            }
            
            // Проверяем, может ли игрок сделать ход
            if (gameStatus.ActivePlayerId != playerId)
            {
                Console.WriteLine("Wrong active player");
                return gameStatus;
            }

            // Обработка хода с камнем
            if (move.Type == GameMove.MoveType.PlaceStone)
            {
                gameStatus.FieldMatrix[move.X][move.Y] = GetPlayerSymbol(playerId, gameStatus);
            }

            // Меняем активного игрока
            gameStatus.ActivePlayerId = gameStatus.ActivePlayerId == gameStatus.Player1Id 
                ? gameStatus.Player2Id 
                : gameStatus.Player1Id;

            // Добавляем ход в историю
            gameStatus.MoveHistory.Add(move);
            gameStatus.CurrentMove++;
            
            return gameStatus;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in ProcessMove: {e}");
            throw;
        }
    }
    
    private char GetPlayerSymbol(Guid playerId, GameStatus gameStatus)
    {
        return playerId == gameStatus.Player1Id ? 'w' : 'b';
    }
}