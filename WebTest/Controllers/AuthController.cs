using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTest.Dtos.Authification;
using WebTest.Interfaces;
using WebTest.Interfaces.Service;
using WebTest.Models;
using WebTest.Servises;

namespace WebTest.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await authService.RegisterAsync(request);

        if (!result.Success)
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.LoginAsync(request);
        
        if (!result.Success)
        {
            return Unauthorized();
        }
        
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authService.RefreshTokenAsync(request);
        
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
    
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(string userId)
    {
        await authService.RevokeTokenAsync(userId);
        return NoContent();
    }
}