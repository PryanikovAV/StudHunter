using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class VacancyController(VacancyService vacancyService) : ControllerBase
{
    private readonly VacancyService _vacancyService = vacancyService;

    [HttpGet]
    public async Task<IActionResult> GetVacancies()
    {
        var vacancies = await _vacancyService.GetVacanciesAsync();

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
    
    [HttpPost]
    public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancyDto dto)
    {
        var (vacancy, error) = await _vacancyService.CreateVacancyAsync(dto);
    
        if (error != null)
            return Conflict(new { error });
    
        return CreatedAtAction(nameof(GetVacancy), new { id = vacancy!.Id }, vacancy);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] UpdateVacancyDto dto)
    {
        var (success, error) = await _vacancyService.UpdateVacancyAsync(id, dto);
    
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
    
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacancy(Guid id)
    {
        var (success, error) = await _vacancyService.DeleteVacancyAsync(id);
            
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
    
        return NoContent();
    }

    [HttpPost("{id}/courses")]
    public async Task<IActionResult> AddCourseToVacancy(Guid id, [FromBody] Guid courseId)
    {
        var (success, error) = await _vacancyService.AddCourseToVacancyAsync(id, courseId);

        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }

    [HttpDelete("{id}/courses/{courseId}")]
    public async Task<IActionResult> RemoveCourseFromVacancy(Guid id, Guid courseId)
    {
        var (success, error) = await _vacancyService.RemoveCourseFromVacancyAsync(id, courseId);

        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }
}
