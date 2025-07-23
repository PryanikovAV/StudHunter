using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminFacultyController(AdminFacultyService adminFacultyService) : ControllerBase
{
    private readonly AdminFacultyService _adminFacultyService = adminFacultyService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var faculty = await _adminFacultyService.GetFacultyAsync(id);
        if (faculty == null)
            return NotFound();
        return Ok(faculty);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (faculty, error) = await _adminFacultyService.CreateFacultyAsync(dto);
        if (faculty == null)
            return error == null ? NotFound() : BadRequest(new { error });
        return CreatedAtAction(nameof(GetFaculty), new { id = faculty!.Id }, faculty);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFaculty(Guid id, [FromBody] UpdateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminFacultyService.UpdateFacultyAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFaculty(Guid id)
    {
        var (success, error) = await _adminFacultyService.DeleteFacultyAsync(id);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
