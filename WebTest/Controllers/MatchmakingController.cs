using Microsoft.AspNetCore.Mvc;
using WebTest.Dtos.Game;
using WebTest.Dtos.User;
using WebTest.Interfaces.Service;

namespace WebTest.Controllers;

[ApiController]
[Route("api/matchmaking")]
public class MatchmakingController(IMatchmakingService matchmakingService) : ControllerBase
{

    [HttpPost("start-search")]
    public async Task<ActionResult<UserResponseDto>> StartSearch([FromBody] GameResponseDto game)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Request"
            });
        }

        var result = await matchmakingService.StartFindMatchmakingAsync(game);

        if (!result.Success)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Requested"
            });
        }

        return Ok(result);
    }
    
    [HttpPost("search")]
    public async Task<ActionResult<UserResponseDto>> Search([FromBody] GameResponseDto game)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Request"
            });
        }

        var result = await matchmakingService.FindMatchmakingAsync(game);

        if (!result.Success)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Requested"
            });
        }

        return Ok(result);
    }

    [HttpPost("cancel")]
    public async Task<ActionResult<bool>> Cancel([FromBody] GameResponseDto game)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Request"
            });
        }

        var result = await matchmakingService.CancelMatchmakingAsync(game);

        return Ok(result);
    }
    
    [HttpPost("start")]
    public async Task<ActionResult<UserResponseDto>> StartMatchmaking(GameResponseDto gameDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Request"
            });
        }

        Console.WriteLine("StartMatchmaking");
        var result = await matchmakingService.StartMatchmakingAsync(gameDto);

        if (!result.Success)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Requested"
            });
        }

        return Ok(result);
    }
}