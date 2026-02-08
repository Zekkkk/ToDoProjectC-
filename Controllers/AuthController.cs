using Microsoft.AspNetCore.Mvc;
using ToDo.Api.DTO.Auth;
using ToDo.Api.Services.Interfaces;

namespace ToDo.Api.Controllers;

// USER NEED: endpoints to register/login and receive JWT tokens.
// DEV: Controller stays thin and maps service errors to HTTP responses.

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
        try
        {
            // USER NEED: create account
            // DEV: delegate to service (hash password + save user)
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            // USER NEED: login and receive JWT token
            // DEV: delegate to service (verify password + create token)
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
