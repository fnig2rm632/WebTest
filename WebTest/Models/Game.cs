using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTest.Models;

public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int BoardSize { get; set; } // 9, 13, 19

    // Игроки
    public string PlayerWhiteId { get; set; }
    public string PlayerBlackId { get; set; }

    // Время
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; } 
    
    // Тип Игры
    
    public string? Type { get; set; }

    public string? WinnerId { get; set; }

    public User PlayerWhite { get; set; }
    public User PlayerBlack { get; set; }
    public User? Winner { get; set; }

}