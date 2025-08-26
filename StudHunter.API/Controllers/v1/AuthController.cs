using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Auth;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : BaseController
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (result, statusCode, errorMessage) = await _authService.LoginAsync(dto);
        return CreateAPIError(result, statusCode, errorMessage);
    }

    [HttpPut("recovery")]
    public async Task<IActionResult> Recovery([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (result, statusCode, errorMessage) = await _authService.RecoverAccountAsync(dto);
        return CreateAPIError(result, statusCode, errorMessage);
    }
}
