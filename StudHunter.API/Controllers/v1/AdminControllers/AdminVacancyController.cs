using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminVacancyController(AdminVacancyService adminVacancyService) : BaseController
{
    private readonly AdminVacancyService _adminVacancyService = adminVacancyService;

    [HttpGet]
    public async Task<IActionResult> GetAllVacancies()
    {
        var (vacancies, statusCode, errorMessage) = await _adminVacancyService.GetAllVacanciesAsync();
        return this.CreateAPIError(vacancies, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVacancy(Guid id)
    {
        var (vacancy, statusCode, errorMessage) = await _adminVacancyService.GetVacancyAsync(id);
        return this.CreateAPIError(vacancy, statusCode, errorMessage);
    }

    [HttpGet("by-employer/{id}")]
    public async Task<IActionResult> GetVacanciesByEmployer(Guid employerId)
    {
        var (vacancies, statusCode, errorMessage) = await _adminVacancyService.GetVacanciesByEmployerAsync(employerId);
        return this.CreateAPIError(vacancies, statusCode, errorMessage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] AdminUpdateVacancyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (vacancy, statusCode, errorMessage) = await _adminVacancyService.UpdateVacancyAsync(id, dto);
        return this.CreateAPIError<AdminVacancyDto>(vacancy, statusCode, errorMessage);
    }

    [HttpPost("{id}/courses")]
    public async Task<IActionResult> AddCourseToVacancy(Guid id, [FromBody] Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (vacancy, statusCode, errorMessage) = await _adminVacancyService.AddCourseToVacancyAsync(id, courseId);
        return this.CreateAPIError<AdminVacancyDto>(vacancy, statusCode, errorMessage);
    }

    [HttpPost("{id}/courses/{courseId}")]
    public async Task<IActionResult> RemoveCourseFromVacancy(Guid id, Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (vacancy, statusCode, errorMessage) = await _adminVacancyService.RemoveCourseFromVacancyAsync(id, courseId);
        return this.CreateAPIError<AdminVacancyDto>(vacancy, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacancy(Guid id)
    {
        var (vacancy, statusCode, errorMessage) = await _adminVacancyService.DeleteVacancyAsync(id);
        return this.CreateAPIError<AdminVacancyDto>(vacancy, statusCode, errorMessage);
    }
}
