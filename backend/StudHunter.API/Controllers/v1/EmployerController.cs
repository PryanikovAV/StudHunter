using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/employers")]
public class EmployerController(IEmployerService employerService) : BaseController
{
    [Authorize]
    [HttpGet("{employerId:guid}/hero")]
    public async Task<IActionResult> GetEmployerHero(Guid employerId) =>
        HandleResult(await employerService.GetEmployerHeroAsync(employerId));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpGet("me/hero")]
    public async Task<IActionResult> GetMyHero() =>
        HandleResult(await employerService.GetEmployerHeroAsync(AuthorizedUserId));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpGet("me/profile")]
    public async Task<IActionResult> GetMyProfile() =>
        HandleResult(await employerService.GetEmployerProfileAsync(AuthorizedUserId));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateEmployerDto dto) =>
        HandleResult(await employerService.UpdateEmployerProfileAsync(AuthorizedUserId, dto));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto) =>
        HandleResult(await employerService.ChangePasswordAsync(AuthorizedUserId, dto));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpPut("me/avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] ChangeAvatarDto dto) =>
        HandleResult(await employerService.UpdateAvatarAsync(AuthorizedUserId, dto));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto) =>
        HandleResult(await employerService.DeleteEmployerAsync(AuthorizedUserId, dto.Password));
}