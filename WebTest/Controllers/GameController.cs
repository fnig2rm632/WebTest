using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTest.Dtos.Friend;
using WebTest.Dtos.Game;
using WebTest.Interfaces.Service;
using WebTest.Models;
using WebTest.Services;

namespace WebTest.Controllers;

[ApiController]
[Route("api/game")]
public class GameController(IGameService gameService) : ControllerBase
{
    [HttpGet("all{userId}")]
    public async Task<ActionResult<ServiceResponse<List<GameResponseDto>>>> GetGameList(string userId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ServiceResponse<List<GameResponseDto>> 
            {
                Success = false,
                Message = "Invalid model state",
                Data = null!
            });
        }
        
        var result = await gameService.GetHistoryList(userId);

        if (!result.Success)
        {
            return Unauthorized(new ServiceResponse<List<GameResponseDto>> 
            {
                Success = false,
                Message = "Unauthorized",
                Data = null!
            });
        }

        return Ok(result);
    }
    
    [HttpPost("game")]
    public async Task<ActionResult<GameResponseDto>> FindGameForData(GameResponseDto responseDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new GameResponseDto
            {
                Id = 0
            });
        }
        
        var result = await gameService.FindGameForDataAsync(responseDto);

        if (result == null)
        {
            return BadRequest(new GameResponseDto
            {
                Id = 0
            });
        }

        return Ok(result);
    }
    
    [HttpPost("finish-game")]
    public async Task<ActionResult<bool>> FinishGameForData(GameResponseDto responseDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(false);
        }
        
        var result = await gameService.UpdateGame(responseDto);

        if (result == null)
        {
            return BadRequest(false);
        }

        return Ok(result);
    }
}