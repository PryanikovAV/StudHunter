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
    public async Task<IActionResult> GetAll() =>
        HandleResult(await adminService.GetAllStudentsAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        HandleResult(await adminService.GetStudentByIdAsync(id));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentDto dto) =>
        HandleResult(await adminService.UpdateStudentAsync(id, dto));

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id) =>
        HandleResult(await adminService.RestoreAsync(id));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] bool hardDelete = false) =>
        HandleResult(await adminService.DeleteStudentAsync(id, hardDelete));
}