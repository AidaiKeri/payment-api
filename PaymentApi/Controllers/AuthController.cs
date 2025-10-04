using Microsoft.AspNetCore.Mvc;
using PaymentApi.DTOs;
using PaymentApi.Services.Interfaces;

namespace PaymentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
            return BadRequest(new { message = "Authorization header is missing" });

        var token = authHeader.Replace("Bearer ", "").Trim();

        var response = await _authService.LogoutAsync(token);

        if (!response)
            return NotFound(new { message = "Session not found or already logged out" });

        return Ok(new { message = "Logged out successfully" });
    }
}

