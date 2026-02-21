using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.API.Infrastructure;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/students")]
public class AdminStudentController(IAdminStudentService adminService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllStudents([FromQuery] PaginationParams paging) =>
        HandleResult(await adminService.GetAllStudentsAsync(paging));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStudentById(Guid id) =>
        HandleResult(await adminService.GetStudentByIdAsync(id));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto) =>
        HandleResult(await adminService.UpdateStudentAsync(id, dto));

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> RestoreStudent(Guid id) =>
        HandleResult(await adminService.RestoreStudentAsync(id));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStudent(Guid id, [FromQuery] bool hardDelete = false) =>
        HandleResult(await adminService.DeleteStudentAsync(id, hardDelete));
}