using System.Security.Cryptography;
using WebTest.Dtos.Game;
using WebTest.Dtos.User;
using WebTest.Interfaces.Manager;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Mappers;
using WebTest.Models;

namespace WebTest.Services;

public class MatchmakingService(
    IGameRepository gameRepository, 
    IUserRepository userRepository, 
    IWedSocketManager wedSocketManager) : IMatchmakingService
{
    public async Task<UserResponseDto> StartFindMatchmakingAsync(GameResponseDto game)
    {
        try
        {
            if (game == null)
            {
                return UserMappers.ToUserResponseDto("Game object is null");
            }

            var requestGame = new Game
            {
                BoardSize = game.BoardSize,
                Type = game.Type,
            };

            if (requestGame == null)
            {
                return UserMappers.ToUserResponseDto("requestGame object is null");
            }

            var reSearch = await gameRepository.FindGameForSearch(requestGame);
            
            if (reSearch == null)
            {
                var x = new Game
                {
                    StartTime = DateTime.Now.ToUniversalTime(),
                    Type = game.Type,
                    BoardSize = game.BoardSize,
                    PlayerWhiteId = game.PlayerWhiteId,
                    PlayerBlackId = "e1856c00-222b-4857-b1ce-c303945b9967",
                };
                
                var i = await gameRepository.AddGame(x);

                return UserMappers.ToUserResponseTrueDto("Created");
            }
            else
            {
                reSearch.PlayerBlackId = game.PlayerWhiteId;
                await gameRepository.UpdateGame(reSearch);
                
                var user = await userRepository.GetByIdAsync(reSearch.PlayerWhiteId);

                return UserMappers.ToUserResponseDto(user!);
            }
            
        }
        catch (Exception e)
        {
            return UserMappers.ToUserResponseDto(e.Message + "\n" + "This");
        }
    }
    
    public async Task<UserResponseDto> FindMatchmakingAsync(GameResponseDto gameDto)
    {
        try
        {
            var game = await gameRepository.FindGameForIdPlayers(gameDto.PlayerWhiteId);

            if (game.PlayerBlackId == "e1856c00-222b-4857-b1ce-c303945b9967")
            {
                return UserMappers.ToUserResponseTrueDto("Matchmaking");
            }
            else
            {
                var user = await userRepository.GetByIdAsync(game.PlayerBlackId);
                
                //await wedSocketManager.TryMatchPlayers( Guid.Parse(gameDto.PlayerWhiteId), Guid.Parse(user.Id),game);
                
                return UserMappers.ToUserResponseDto(user!);
                
            }
        }
        catch (Exception e)
        {
            return UserMappers.ToUserResponseDto(e.Message + "\n" + "This");
        }
    }
    
    public async Task<UserResponseDto>StartMatchmakingAsync(GameResponseDto gameDto)
    {
        Console.WriteLine("StartMatchmaking");
        try
        {
            var game = await gameRepository.FindGameForIdPlayers(gameDto.PlayerWhiteId);

            if (game.PlayerBlackId == "e1856c00-222b-4857-b1ce-c303945b9967")
            {
                return UserMappers.ToUserResponseTrueDto("Matchmaking");
            }
            else
            {
                var user = await userRepository.GetByIdAsync(game.PlayerBlackId);
                /*var success = await wedSocketManager.TryMatchPlayers( Guid.Parse(gameDto.PlayerWhiteId), Guid.Parse(user.Id),game);

                if (!success)
                {
                    return UserMappers.ToUserResponseTrueDto("Close");
                }*/

                return UserMappers.ToUserResponseDto(user!);
            }
        }
        catch (Exception e)
        {
            return UserMappers.ToUserResponseDto(e.Message + "\n" + "This");
        }
    }
    
    public async Task<bool> CancelMatchmakingAsync(GameResponseDto gameDto)
    {
        try
        {
            var game = await gameRepository.FindGameForIdPlayers(gameDto.PlayerWhiteId, "e1856c00-222b-4857-b1ce-c303945b9967");
            return await gameRepository.DeleteGame(game);
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
}