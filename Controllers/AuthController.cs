using Microsoft.AspNetCore.Mvc;
using ToDo.Api.DTO.Auth;
using ToDo.Api.Services.Interfaces;

namespace ToDo.Api.Controllers;

// USER NEED: endpoints to register/login
// DEV: Controller should be thin -> calls the service

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        // USER NEED: create account
        // DEV: delegate to service (hash password + save user)
        var result = await _authService.RegisterAsync(dto);

        return Ok(result);
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        // USER NEED: login and receive JWT token
        // DEV: delegate to service (verify password + create token)
        var result = await _authService.LoginAsync(dto);

        return Ok(result);
    }
}