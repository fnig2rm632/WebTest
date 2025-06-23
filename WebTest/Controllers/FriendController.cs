using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTest.Dtos.Friend;
using WebTest.Interfaces;
using WebTest.Interfaces.Service;
using WebTest.Models;

namespace WebTest.Controllers;

[Authorize]
[ApiController]
[Route("api/friends")]
public class FriendController(IFriendService friendService) : ControllerBase
{
    [HttpGet("all{userId}")]
    public async Task<ActionResult<ServiceResponse<List<FriendResponseDto>>>> GetFriendsList(string userId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ServiceResponse<List<FriendResponseDto>> 
            {
                Success = false,
                Message = "Invalid model state",
                Data = null!
            });
        }
        
        var result = await friendService.GetFriendsList(userId);

        if (!result.Success)
        {
            return Unauthorized(new ServiceResponse<List<FriendResponseDto>> 
            {
                Success = false,
                Message = "Unauthorized",
                Data = null!
            });
        }

        return Ok(result);
    }
    
    [HttpPost("find-friend")]
    public async Task<ActionResult<ServiceResponse<List<FriendResponseDto>>>> FindFriendsList([FromBody] FriendResponseDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ServiceResponse<List<FriendResponseDto>> 
            {
                Success = false,
                Message = "Invalid model state",
                Data = null!
            });
        }
        
        var result = await friendService.SearchFriends(request.Query,request.MyId);

        if (!result.Success)
        {
            return Unauthorized(new ServiceResponse<List<FriendResponseDto>> 
            {
                Success = false,
                Message = "Unauthorized",
                Data = null!
            });
        }

        return Ok(result);
    }

    [HttpPost("send-request")]
    public async Task<ActionResult<ServiceResponse<bool?>>> SendFriendRequest([FromBody] FriendResponseDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ServiceResponse<bool?> 
            {
                Success = false,
                Message = "Invalid model state",
                Data = null
            });
        }
        
        var result = await friendService.SendFriendRequest(request.FriendId, request.MyId);
        
        if (!result.Success)
        {
            return Unauthorized(new ServiceResponse<bool?> 
            {
                Success = false,
                Message = "Unauthorized",
                Data = null
            });
        }
        
        return Ok(result);
    }
    
    [HttpPost("accept-request")]
    public async Task<ActionResult<ServiceResponse<bool?>>> AcceptFriendRequest([FromBody] FriendResponseDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ServiceResponse<bool?> 
            {
                Success = false,
                Message = "Invalid model state",
                Data = null
            });
        }
        
        var result = await friendService.AcceptFriendRequest(request.FriendId, request.MyId);
        
        if (!result.Success)
        {
            return Unauthorized(new ServiceResponse<bool?> 
            {
                Success = false,
                Message = "Unauthorized",
                Data = null
            });
        }
        
        return Ok(result);
    }
    
    [HttpDelete("delete-request")]
    public async Task<ActionResult<ServiceResponse<bool?>>> DeleteFriendRequest([FromBody] FriendResponseDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ServiceResponse<bool?> 
            {
                Success = false,
                Message = "Invalid model state",
                Data = null
            });
        }

        var result = await friendService.DeleteFriend(request.FriendId, request.MyId);
        
        if (!result.Success)
        {
            return Unauthorized(new ServiceResponse<bool?> 
            {
                Success = false,
                Message = "Unauthorized",
                Data = null
            });
        }
        
        return Ok(result);
    }
}