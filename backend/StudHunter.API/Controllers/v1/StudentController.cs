using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;
using StudHunter.API.Infrastructure;

namespace StudHunter.API.Controllers.v1;

[Authorize(Roles = UserRoles.Student)]
public class StudentController(IStudentService studentService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetProfile() =>
        HandleResult(await studentService.GetStudentAsync(AuthorizedUserId));

    [HttpGet("hero")]
    public async Task<IActionResult> GetStudentHero() =>
        HandleResult(await studentService.GetStudentHeroAsync(AuthorizedUserId));

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateStudentDto dto) =>
        HandleResult(await studentService.UpdateStudentAsync(AuthorizedUserId, dto));

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount() =>
        HandleResult(await studentService.DeleteStudentAsync(AuthorizedUserId));
}