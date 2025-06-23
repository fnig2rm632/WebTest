namespace WebTest.Models;

public class GameMove
{
    /// <summary>
    /// Тип хода: размещение камня, пас или сдача
    /// </summary>
    public MoveType Type { get; set; }

    /// <summary>
    /// Координата X на доске (0-18 для стандартной доски 19x19)
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Координата Y на доске (0-18 для стандартной доски 19x19)
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Цвет камня (для хода типа PlaceStone)
    /// </summary>
    public StoneColor Color { get; set; }

    /// <summary>
    /// ID игрока
    /// </summary>
    public Guid IdPlayer { get; set; }
    
    
    public int IdGame { get; set; }

    /// <summary>
    /// Дополнительные данные (например, комментарий к ходу)
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    public enum MoveType
    {
        PlaceStone, // Размещение камня
        Pass,       // Пас (пропуск хода)
        Resign      // Сдача
    }

    public enum StoneColor
    {
        Black,
        White
    }

    /// <summary>
    /// Создает ход с размещением камня
    /// </summary>
    public static GameMove CreatePlaceMove(int x, int y, StoneColor color, Guid idPlayer, int idGame)
    {
        return new GameMove
        {
            Type = MoveType.PlaceStone,
            IdPlayer = idPlayer,
            IdGame = idGame,
            X = x,
            Y = y,
            Color = color
        };
    }

    /// <summary>
    /// Создает ход "пас"
    /// </summary>
    public static GameMove CreatePassMove(StoneColor color, Guid idPlayer)
    {
        return new GameMove
        {
            Type = MoveType.Pass,
            IdPlayer = idPlayer,
            Color = color
        };
    }

    /// <summary>
    /// Создает ход "сдача"
    /// </summary>
    public static GameMove CreateResignMove(StoneColor color, Guid idPlayer)
    {
        return new GameMove
        {
            Type = MoveType.Resign,
            IdPlayer = idPlayer,
            Color = color
        };
    }

    public override string ToString()
    {
        return Type switch
        {
            MoveType.PlaceStone => $"{Color} stone at ({X},{Y})",
            MoveType.Pass => $"{Color} passes",
            MoveType.Resign => $"{Color} resigns",
            _ => "Unknown move"
        };
    }
}