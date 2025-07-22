using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminVacancyController(AdminVacancyService adminVacancyService) : ControllerBase
{
    private readonly AdminVacancyService _adminVacancyService = adminVacancyService;

    [HttpGet]
    public async Task<IActionResult> GetVacancies()
    {
        var vacancies = await _adminVacancyService.GetAllVacanciesAsync();
        return Ok(vacancies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetVacancy(Guid id)
    {
        var vacancy = await _adminVacancyService.GetVacancyAsync(id);
        if (vacancy == null)
            return NotFound();
        return Ok(vacancy);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] UpdateVacancyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminVacancyService.UpdateVacancyAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacancy(Guid id)
    {
        var (success, error) = await _adminVacancyService.DeleteVacancyAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpPost("{id}/courses")]
    public async Task<IActionResult> AddCourseToVacancy(Guid id, [FromBody] Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminVacancyService.AddCourseToVacancyAsync(id, courseId);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpPost("{id}/courses/{courseId}")]
    public async Task<IActionResult> RemoveCourseFromVacancy(Guid id, Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminVacancyService.RemoveCourseFromVacancyAsync(id, courseId);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
