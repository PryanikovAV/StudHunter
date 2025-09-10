using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.AuthDto;
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
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<LoginDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (result, statusCode, errorMessage) = await _authService.LoginAsync(dto);
        return HandleResponse(result, statusCode, errorMessage);
    }

    [HttpPut("recovery")]
    public async Task<IActionResult> Recovery([FromBody] LoginDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (result, statusCode, errorMessage) = await _authService.RecoverAccountAsync(dto);
        return HandleResponse(result, statusCode, errorMessage);
    }
}
