using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Administrator;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class AdministratorController(AdministratorService administratorService) : ControllerBase
{
    private readonly AdministratorService _administratorService = administratorService;

    [HttpGet]
    public async Task<IActionResult> GetAdministrators()
    {
        var administrators = await _administratorService.GetAdministratorsAsync();

        return Ok(administrators);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAdministrator(Guid id)
    {
        var administrator = await _administratorService.GetAdministratorAsync(id);

        if (administrator == null)
            return NotFound();

        return Ok(administrator);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdministrator([FromBody] CreateAdministratorDto dto)
    {
        var (administrator, error) = await _administratorService.CreateAdministratorAsync(dto);

        if (error != null)
            return Conflict(new { error });

        return CreatedAtAction(nameof(GetAdministrator), new { id = administrator!.Id }, administrator);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdministrator(Guid id, [FromBody] UpdateAdministratorDto dto)
    {
        var (success, error) = await _administratorService.UpdateAdministratorAsync(id, dto);

        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdministrator(Guid id)
    {
        var (success, error) = await _administratorService.DeleteAdministratorAsync(id);

        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("employer/{id}/accreditation")]
    public async Task<IActionResult> UpdateEmployerAccreditation(Guid id, [FromBody] UpdateEmployerAccreditationDto dto)
    {
        var (success, error) = await _administratorService.UpdateEmployerAccreditationAsync(id, dto.AccreditationStatus);
        
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("student/{id}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
    {
        var (success, error) = await _administratorService.UpdateStudentAsync(id, dto);
        
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("employer/{id}")]
    public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] UpdateEmployerDto dto)
    {
        var (success, error) = await _administratorService.UpdateEmployerAsync(id, dto);
        
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("resume/{id}")]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] UpdateResumeDto dto)
    {
        var (success, error) = await _administratorService.UpdateResumeAsync(id, dto);
        
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("vacancy/{id}")]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] UpdateVacancyDto dto)
    {
        var (success, error) = await _administratorService.UpdateVacancyAsync(id, dto);
        
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites()
    {
        var favorites = await _administratorService.GetFavoritesAsync();
        return Ok(favorites);
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("favorites/{id}")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var (success, error) = await _administratorService.DeleteFavoriteAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}