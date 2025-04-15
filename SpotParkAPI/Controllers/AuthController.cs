using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services.Interfaces;

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
        
            var token = await _authService.LoginAsync(request);
            return Ok(new { Token = token });
       

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
      
            var result = await _authService.RegisterAsync(request);
            return Ok(new { Success = result });
          
    }
}