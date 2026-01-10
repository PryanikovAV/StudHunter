using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.API.Infrastructure;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/employers")]
public class AdminEmployerController(IAdminEmployerService adminService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        HandleResult(await adminService.GetAllEmployersAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        HandleResult(await adminService.GetEmployerByIdAsync(id));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployerDto dto) =>
        HandleResult(await adminService.UpdateEmployerAsync(id, dto));

    [HttpPatch("{id:guid}/verify")]
    public async Task<IActionResult> Verify(Guid id, [FromQuery] bool isVerified) =>
        HandleResult(await adminService.VerifyEmployerAsync(id));

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id) =>
        HandleResult(await adminService.RestoreAsync(id));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] bool hardDelete = false) =>
        HandleResult(await adminService.DeleteEmployerAsync(id, hardDelete));
}