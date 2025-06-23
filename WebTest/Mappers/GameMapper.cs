using WebTest.Dtos.Game;
using WebTest.Models;

namespace WebTest.Mappers;

public static class GameMapper
{
    public static GameResponseDto ToGameResponseDto(Game game)
    {
        return new GameResponseDto()
        {
            Id = game.Id,
            BoardSize = game.BoardSize,
            PlayerWhiteId = game.PlayerWhiteId,
            PlayerBlackId = game.PlayerBlackId,
            StartTime = game.StartTime,
            EndTime = game.EndTime,
            Type = game.Type
        };
    }
    
    public static GameResponseDto ToGameSearchResponseDto(Game game)
    {
        return new GameResponseDto()
        {
            BoardSize = game.BoardSize,
            PlayerWhiteId = game.PlayerWhiteId,
            Type = game.Type
        };
    }
    
    
    public static List<GameResponseDto> ToGameResponseListDto(List<Game> games)
    {
        return games.Select(ToGameResponseDto).ToList();
    }
}