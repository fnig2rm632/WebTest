using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTest.Dtos.User;
using WebTest.Interfaces.Service;

namespace WebTest.Controllers;
[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("user{userId}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(string userId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Request"
            });
        }
        
        var result = await userService.GetUser(userId);

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
    
    [HttpGet("profile{userId}")]
    public async Task<ActionResult<UserResponseDto>> GetProfileUser(string userId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Error = "Bad Request"
            });
        }
        
        var result = await userService.GetUserProfile(userId);

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