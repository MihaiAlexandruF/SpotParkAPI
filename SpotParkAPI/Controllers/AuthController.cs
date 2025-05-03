using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services.Interfaces;
using System.Security.Claims;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {

        Console.WriteLine("Login request received: UsernameOrEmail = " + request.UsernameOrEmail);
        var result = await _authService.LoginAsync(request);

        Console.WriteLine("Authentication successful for: " + request.UsernameOrEmail);
        return Ok(result);
       

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
      
            var result = await _authService.RegisterAsync(request);
            return Ok(new { Success = result });
          
    }

    [HttpGet("validate")]
    [Authorize] 
    public IActionResult Validate()
    {
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            Valid = true,
            UserId = userId,
            Username = username
        });
    }
}