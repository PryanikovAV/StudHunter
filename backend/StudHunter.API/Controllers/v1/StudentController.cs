using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/students")]
public class StudentController(IStudentService studentService) : BaseController
{
    [Authorize]
    [HttpGet("{studentId:guid}/hero")]
    public async Task<IActionResult> GetStudentHero(Guid studentId) =>
        HandleResult(await studentService.GetStudentHeroAsync(studentId));

    [Authorize(Roles = UserRoles.Student)]
    [HttpGet("me/hero")]
    public async Task<IActionResult> GetMyHero() =>
        HandleResult(await studentService.GetStudentHeroAsync(AuthorizedUserId));

    [Authorize(Roles = UserRoles.Student)]
    [HttpGet("me/profile")]
    public async Task<IActionResult> GetMyProfile() =>
        HandleResult(await studentService.GetStudentProfileAsync(AuthorizedUserId));

    [Authorize(Roles = UserRoles.Student)]
    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateStudentDto dto) =>
        HandleResult(await studentService.UpdateStudentProfileAsync(AuthorizedUserId, dto));

    [Authorize(Roles = UserRoles.Student)]
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto) =>
        HandleResult(await studentService.ChangePasswordAsync(AuthorizedUserId, dto));

    [Authorize(Roles = UserRoles.Student)]
    [HttpPut("me/avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] ChangeAvatarDto dto) =>
        HandleResult(await studentService.UpdateAvatarAsync(AuthorizedUserId, dto));

    [Authorize(Roles = UserRoles.Student)]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto) =>
        HandleResult(await studentService.DeleteStudentAsync(AuthorizedUserId, dto.Password));
}