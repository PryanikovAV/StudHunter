using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class VacancyController(VacancyService vacancyService) : ControllerBase
{
    private readonly VacancyService _vacancyService = vacancyService;

    [HttpGet]
    public async Task<IActionResult> GetAllVacancies()
    {
        var vacancies = await _vacancyService.GetAllVacanciesAsync();
        return Ok(vacancies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetVacancy(Guid id)
    {
        var vacancy = await _vacancyService.GetVacancyAsync(id);
        if (vacancy == null)
            return NotFound();
        return Ok(vacancy);
    }

    /// <summary>
    /// Retrieves all vacancies for a specific employer.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <returns>A collection of vacancies associated with the employer.</returns>
    [HttpGet("employer/{employerId}/vacancies")]
    public async Task<IActionResult> GetVacanciesByEmployer(Guid employerId)
    {
        var vacancies = await _vacancyService.GetVacanciesByEmployerAsync(employerId);
        return Ok(vacancies);
    }

    // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
    [HttpPost]
    public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var employerId = Guid.NewGuid();
        var (vacancy, error) = await _vacancyService.CreateVacancyAsync(employerId, dto);
        if (vacancy == null)
            return error == null ? NotFound() : BadRequest(new { error });
        return CreatedAtAction(nameof(GetVacancy), new { id = vacancy.Id }, vacancy);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] UpdateVacancyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _vacancyService.UpdateVacancyAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpPost("{id}/courses")]
    public async Task<IActionResult> AddCourseToVacancy(Guid id, [FromBody] Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _vacancyService.AddCourseToVacancyAsync(id, courseId);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpPost("{id}/courses/{courseId}")]
    public async Task<IActionResult> RemoveCourseFromVacancy(Guid id, Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _vacancyService.RemoveCourseFromVacancyAsync(id, courseId);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacancy(Guid id)
    {
        var (success, error) = await _vacancyService.DeleteVacancyAsync(id);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
