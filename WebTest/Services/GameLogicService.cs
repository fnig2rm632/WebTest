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
                var gameService = scope.ServiceProvider.GetRequiredService<IGameRepository>();
                
                var winnerId = gameStatus.Player1Id == playerId 
                    ? gameStatus.Player2Id 
                    : gameStatus.Player1Id;
                
                gameStatus.WinPlayer = winnerId;
                
                var item = await gameService.FindGameForIdPlayersElse(gameStatus.Player1Id.ToString(), gameStatus.Player2Id.ToString());

                item.WinnerId = winnerId.ToString();
                item.EndTime = DateTime.Now.ToUniversalTime();
                gameService.UpdateGame(item);
                
                _activeGames.TryRemove(move.IdGame, out _);
                return gameStatus;
            }
            
            // Проверяем, может ли игрок сделать ход
            if (gameStatus.ActivePlayerId != playerId)
            {
                Console.WriteLine("Wrong active player");
                return gameStatus;
            }
            
            if (move.Type == GameMove.MoveType.Pass)
            {
                gameStatus.ConsecutivePasses++;

                if (gameStatus.ConsecutivePasses >= 2)
                {
                    // Подсчёт очков
                    var (score1, score2) = CalculateScore(gameStatus);

                    if (score1 > score2)
                        gameStatus.WinPlayer = gameStatus.Player1Id;
                    else if (score2 > score1)
                        gameStatus.WinPlayer = gameStatus.Player2Id;
                    else
                        gameStatus.WinPlayer = gameStatus.Player2Id; // Ничья

                    using var scope = _serviceProvider.CreateScope();
                    var gameService = scope.ServiceProvider.GetRequiredService<IGameRepository>();
                    
                    var item = await gameService.FindGameForIdPlayersElse(gameStatus.Player1Id.ToString(), gameStatus.Player2Id.ToString());

                    item.WinnerId = gameStatus.WinPlayer.ToString();
                    item.EndTime = DateTime.Now.ToUniversalTime();
                    gameService.UpdateGame(item);
                }
            }
            else
            {
                gameStatus.ConsecutivePasses = 0;
            }

            if (move.Type == GameMove.MoveType.PlaceStone)
            {
                gameStatus.FieldMatrix[move.X][move.Y] = GetPlayerSymbol(playerId, gameStatus);

                // Проверяем соседей — удаляем захваченные камни противника
                var captured = RemoveCapturedGroups(gameStatus, move.X, move.Y, OpponentSymbol(playerId, gameStatus));

                // Увеличиваем счётчик
                if (playerId == gameStatus.Player1Id)
                    gameStatus.CapturedByPlayer1 += captured;
                else
                    gameStatus.CapturedByPlayer2 += captured;
                
            }

            // Меняем активного игрока
            gameStatus.ActivePlayerId = gameStatus.ActivePlayerId == gameStatus.Player1Id 
                ? gameStatus.Player2Id 
                : gameStatus.Player1Id;

            // Добавляем ход в историю
            gameStatus.MoveHistory.Add(move);
            gameStatus.CurrentMove++;
            
            // Реализация логики игры
            return gameStatus;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in ProcessMove: {e}");
            throw;
        }
    }
    
    private (int, int) CalculateScore(GameStatus gameStatus)
    {
        int score1 = gameStatus.CapturedByPlayer1;
        int score2 = gameStatus.CapturedByPlayer2;

        for (int x = 0; x < gameStatus.FieldMatrix.Count; x++)
        {
            for (int y = 0; y < gameStatus.FieldMatrix.Count; y++)
            {
                if (gameStatus.FieldMatrix[x][y] == 'w')
                    score2++;
                else if (gameStatus.FieldMatrix[x][y] == 'b')
                    score1++;
                // пустые клетки не считаются — можешь позже добавить проверку территории
            }
        }

        return (score1, score2);
    }
    
    private char OpponentSymbol(Guid playerId, GameStatus gameStatus)
    {
        return playerId == gameStatus.Player1Id ? 'b' : 'w';
    }
    
    private int RemoveCapturedGroups(GameStatus gameStatus, int x, int y, char targetSymbol)
    {
        var directions = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
        var visited = new HashSet<(int, int)>();
        int captured = 0;

        foreach (var (dx, dy) in directions)
        {
            int nx = x + dx, ny = y + dy;
            if (!InBounds(gameStatus, nx, ny) || gameStatus.FieldMatrix[nx][ny] != targetSymbol)
                continue;

            var group = new List<(int, int)>();
            if (!HasLiberties(gameStatus, nx, ny, targetSymbol, group, new HashSet<(int, int)>()))
            {
                foreach (var (gx, gy) in group)
                    gameStatus.FieldMatrix[gx][gy] = '\0';
                captured += group.Count;
            }
        }

        return captured;
    }

    private bool HasLiberties(GameStatus gameStatus, int x, int y, char symbol, List<(int, int)> group, HashSet<(int, int)> visited)
    {
        if (!InBounds(gameStatus, x, y) || visited.Contains((x, y)))
            return false;

        if (gameStatus.FieldMatrix[x][y] == '\0') return true; // liberty

        if (gameStatus.FieldMatrix[x][y] != symbol) return false;

        visited.Add((x, y));
        group.Add((x, y));

        foreach (var (dx, dy) in new[] { (0, 1), (1, 0), (0, -1), (-1, 0) })
        {
            int nx = x + dx, ny = y + dy;
            if (HasLiberties(gameStatus, nx, ny, symbol, group, visited))
                return true;
        }

        return false;
    }

    private bool InBounds(GameStatus gameStatus, int x, int y)
    {
        return x >= 0 && y >= 0 && x < gameStatus.FieldMatrix.Count && y < gameStatus.FieldMatrix.Count;
    }
    
    private bool IsValidMove(GameStatus status, int x, int y)
    {
        return x >= 0 && y >= 0 &&
               x < status.FieldMatrix.Count &&
               y < status.FieldMatrix[x].Count &&
               status.FieldMatrix[x][y] == '\0';
    }

    private List<(int, int)> GetAdjacentPoints(int x, int y, List<List<char>> board)
    {
        var directions = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
        var result = new List<(int, int)>();
        foreach (var (dx, dy) in directions)
        {
            int nx = x + dx, ny = y + dy;
            if (nx >= 0 && ny >= 0 && nx < board.Count && ny < board[nx].Count)
            {
                result.Add((nx, ny));
            }
        }
        return result;
    }

    private bool HasLiberty(int x, int y, List<List<char>> board, HashSet<(int, int)> visited)
    {
        var symbol = board[x][y];
        if (symbol == '\0') return true;

        visited.Add((x, y));
        foreach (var (nx, ny) in GetAdjacentPoints(x, y, board))
        {
            if (board[nx][ny] == '\0') return true;
            if (board[nx][ny] == symbol && !visited.Contains((nx, ny)))
            {
                if (HasLiberty(nx, ny, board, visited)) return true;
            }
        }

        return false;
    }

    private void RemoveGroup(HashSet<(int, int)> group, List<List<char>> board)
    {
        foreach (var (x, y) in group)
        {
            board[x][y] = '\0';
        }
    }
    
    private char GetPlayerSymbol(Guid playerId, GameStatus gameStatus)
    {
        return playerId == gameStatus.Player1Id ? 'w' : 'b';
    }
}