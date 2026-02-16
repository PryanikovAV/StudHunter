using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.API.Infrastructure;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/study-plans")]
public class AdminStudyPlanController(IAdminStudyPlanService adminService) : BaseController
{
    [HttpGet("{studentId:guid}")]
    public async Task<IActionResult> GetStudentStudyPlan(Guid studentId) =>
        HandleResult(await adminService.GetStudyPlanByStudentIdAsync(studentId));

    [HttpPost("{studentId:guid}")]
    public async Task<IActionResult> CreateForStudent(Guid studentId, [FromBody] UpdateStudyPlanDto dto) =>
        HandleResult(await adminService.CreateAsync(studentId, dto));

    [HttpPut("{studentId:guid}")]
    public async Task<IActionResult> UpdateForStudent(Guid studentId, [FromBody] UpdateStudyPlanDto dto) =>
        HandleResult(await adminService.UpdateStudyPlanAsync(studentId, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] bool hardDelete = false) =>
        HandleResult(await adminService.DeleteAsync(id, hardDelete));
}