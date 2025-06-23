namespace WebTest.Models;

public class GameStatus
{
    public int GameId { get; set; }
    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }
    public List<List<char>> FieldMatrix { get; set; } 
    public List<GameMove> MoveHistory { get; set; } 
    public Guid ActivePlayerId { get; set; } 
    public Guid? WinPlayer { get; set; }
    public int CurrentMove { get; set; } 
    public int CapturedByPlayer1 { get; set; } = 0;
    public int CapturedByPlayer2 { get; set; } = 0;
    public int ConsecutivePasses { get; set; } = 0;
}