using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/vacancies")]
public class AdminVacancyController(IAdminVacancyService adminVacancyService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams paging) =>
        HandleResult(await adminVacancyService.GetAllVacanciesAsync(paging));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        HandleResult(await adminVacancyService.GetVacancyByIdAsync(id));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] VacancyFillDto dto) =>
        HandleResult(await adminVacancyService.UpdateVacancyAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] bool hardDelete = false) =>
        HandleResult(await adminVacancyService.DeleteVacancyAsync(id, hardDelete));

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id) =>
        HandleResult(await adminVacancyService.RestoreVacancyAsync(id));
}