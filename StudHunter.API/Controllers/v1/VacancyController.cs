using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class VacancyController(VacancyService vacancyService) : BaseController
{
    private readonly VacancyService _vacancyService = vacancyService;

    [HttpGet]
    public async Task<IActionResult> GetAllVacancies()
    {
        var (vacancies, statusCode, errorMessage) = await _vacancyService.GetAllVacanciesAsync();
        return this.CreateAPIError(vacancies, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVacancy(Guid id)
    {
        var (vacancy, statusCode, errorMessage) = await _vacancyService.GetVacancyAsync(id);
        return this.CreateAPIError(vacancy, statusCode, errorMessage);
    }

    [HttpGet("employer/{employerId}/vacancies")]
    public async Task<IActionResult> GetVacanciesByEmployer(Guid employerId)
    {
        var (vacancies, statusCode, errorMessage) = await _vacancyService.GetVacanciesByEmployerAsync(employerId);
        return this.CreateAPIError(vacancies, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var employerId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (vacancy, statusCode, errorMessage) = await _vacancyService.CreateVacancyAsync(employerId, dto);
        return this.CreateAPIError(vacancy, statusCode, errorMessage, nameof(GetVacancy), new { id = vacancy?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] UpdateVacancyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (vacancy, statusCode, errorMessage) = await _vacancyService.UpdateVacancyAsync(id, dto);
        return this.CreateAPIError<VacancyDto>(vacancy, statusCode, errorMessage);
    }

    [HttpPost("{id}/courses")]
    public async Task<IActionResult> AddCourseToVacancy(Guid id, [FromBody] Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (vacancy, statusCode, errorMessage) = await _vacancyService.AddCourseToVacancyAsync(id, courseId);
        return this.CreateAPIError<VacancyDto>(vacancy, statusCode, errorMessage);
    }

    [HttpPost("{id}/courses/{courseId}")]
    public async Task<IActionResult> RemoveCourseFromVacancy(Guid id, Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (vacancy, statusCode, errorMessage) = await _vacancyService.RemoveCourseFromVacancyAsync(id, courseId);
        return this.CreateAPIError<VacancyDto>(vacancy, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacancy(Guid id)
    {
        var (vacancy, statusCode, errorMessage) = await _vacancyService.DeleteVacancyAsync(id);
        return this.CreateAPIError<VacancyDto>(vacancy, statusCode, errorMessage);
    }
}
