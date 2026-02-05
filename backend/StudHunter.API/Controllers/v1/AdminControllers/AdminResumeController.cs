using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/resumes")]
public class AdminResumeController(IAdminResumeService adminResumeService) : BaseController
{
    [HttpGet("{studentId:guid}")]
    public async Task<IActionResult> GetByStudentId(Guid studentId) =>
        HandleResult(await adminResumeService.GetResumeByStudentIdAsync(studentId, null, true));

    [HttpPut("{studentId:guid}")]
    public async Task<IActionResult> UpdateResume(Guid studentId, [FromBody] UpdateResumeDto dto) =>
        HandleResult(await adminResumeService.UpdateResumeAsync(studentId, dto));

    [HttpPost("{studentId:guid}/restore")]
    public async Task<IActionResult> RestoreResume(Guid studentId) =>
        HandleResult(await adminResumeService.RestoreResumeAsync(studentId));

    [HttpDelete("{studentId:guid}/soft")]
    public async Task<IActionResult> SoftDelete(Guid studentId) =>
        HandleResult(await adminResumeService.SoftDeleteResumeAsync(studentId));

    [HttpDelete("{studentId:guid}/hard")]
    public async Task<IActionResult> HardDelete(Guid studentId) =>
        HandleResult(await adminResumeService.HardDeleteResumeAsync(studentId));
}
