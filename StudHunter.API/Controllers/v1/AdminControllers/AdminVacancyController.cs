using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/vacancies")]
public class AdminVacancyController(
    IVacancyService vacancyService,
    IAdminVacancyService adminVacancyService) : BaseController
{
    [HttpGet("employer/{employerId:guid}")]
    public async Task<IActionResult> GetVacanciesByEmployer(Guid employerId) =>
        HandleResult(await adminVacancyService.GetAllVacanciesAsync(employerId));

    [HttpPost("employer/{employerId:guid}")]
    public async Task<IActionResult> Create(Guid employerId, [FromBody] UpdateVacancyDto dto) =>
        HandleResult(await vacancyService.CreateVacancyAsync(employerId, dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVacancyDto dto) =>
        HandleResult(await adminVacancyService.UpdateVacancyAsync(id, dto));

    [HttpDelete("{id:guid}/soft")]
    public async Task<IActionResult> SoftDelete(Guid id) =>
        HandleResult(await adminVacancyService.SoftDeleteVacancyAsync(id));

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id) =>
        HandleResult(await adminVacancyService.RestoreVacancyAsync(id));

    [HttpDelete("{id:guid}/hard")]
    public async Task<IActionResult> HardDelete(Guid id) =>
        HandleResult(await adminVacancyService.HardDeleteVacancyAsync(id));
}