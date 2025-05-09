using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services;
using SpotParkAPI.Services.Interfaces;
using System.Security.Claims;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICommonService _commonService;

    public AuthController(IAuthService authService,ICommonService commonService)
    {
        _authService = authService;
        _commonService = commonService;
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
    public async Task<IActionResult> Validate()
    {
        var userId = _commonService.GetCurrentUserId();


        var userValidation = await _authService.GetUserValidationDtoAsync(userId);

        return Ok(userValidation);
    }





}