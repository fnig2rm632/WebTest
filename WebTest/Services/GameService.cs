using WebTest.Dtos.Game;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Mappers;
using WebTest.Models;

namespace WebTest.Services;

public class GameService(IGameRepository gameRepository, IUserRepository userRepository) : IGameService
{
    // Взять лист гейм
    public override async Task<ServiceResponse<List<GameResponseDto>>> GetHistoryList(string userId)
    {
        var response = new ServiceResponse<List<GameResponseDto>>();
            
        try
        {
            var games = GameMapper.ToGameResponseListDto(await gameRepository.GetGamesByIdAsync(userId));;

            foreach (var game in games)
            {
                game.PlayerBlackName = userRepository.GetByIdAsync(game.PlayerBlackId).Result!.UserName;
                game.PlayerWhiteName = userRepository.GetByIdAsync(game.PlayerWhiteId).Result!.UserName;
            }
            
            response.Data = games.ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }


    public override async Task<GameResponseDto?> FindGameForDataAsync(GameResponseDto responseDto)
    {
        var response = new GameResponseDto();

        try
        {
            var game = await gameRepository.FindCreateGameForSearch(responseDto.PlayerWhiteId);


            if (game != null)
            {
                response = GameMapper.ToGameResponseDto(game);
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return response;
    }

    public override async Task<bool> UpdateGame(GameResponseDto responseDto)
    {
        try
        {
            var game = await gameRepository.FindGameForIdPlayersElse(responseDto.PlayerWhiteId, responseDto.PlayerBlackId);
            
            game.WinnerId = responseDto.PlayerWinId;
            game.EndTime = DateTime.Now.ToUniversalTime();
            
            var isUpdate = await gameRepository.UpdateGame(game);

            return isUpdate;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}