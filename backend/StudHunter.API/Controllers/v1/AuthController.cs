using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AuthService;

namespace StudHunter.API.Controllers.v1;

[AllowAnonymous]
public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) =>
        HandleResult(await authService.LoginAsync(dto));

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto dto) =>
        HandleResult(await authService.RegisterStudentAsync(dto));

    [HttpPost("register/employer")]
    public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDto dto) =>
        HandleResult(await authService.RegisterEmployerAsync(dto));

    [HttpPost("recover")]
    public async Task<IActionResult> Recover([FromBody] LoginDto dto) =>
        HandleResult(await authService.RecoverAccountAsync(dto));
}