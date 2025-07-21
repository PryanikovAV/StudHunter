using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdministratorFacultyController(AdministratorFacultyService administratorFacultyService) : ControllerBase
{
    private readonly AdministratorFacultyService _administratorFacultyService = administratorFacultyService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var faculty = await _administratorFacultyService.GetFacultyAsync(id);
        if (faculty == null)
            return NotFound();
        return Ok(faculty);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (faculty, error) = await _administratorFacultyService.CreateFacultyAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetFaculty), new { id = faculty!.Id }, faculty);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFaculty(Guid id, [FromBody] UpdateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _administratorFacultyService.UpdateFacultyAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFaculty(Guid id)
    {
        var (success, error) = await _administratorFacultyService.DeleteFacultyAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
