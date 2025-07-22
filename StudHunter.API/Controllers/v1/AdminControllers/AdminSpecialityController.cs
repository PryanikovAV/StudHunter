using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminSpecialityController(AdminSpecialityService adminSpecialityService) : ControllerBase
{
    private readonly AdminSpecialityService _adminSpecialityService = adminSpecialityService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpeciality(Guid id)
    {
        var speciality = await _adminSpecialityService.GetSpecialityAsync(id);
        if (speciality == null)
            return NotFound();
        return Ok(speciality);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpeciality([FromBody] CreateSpecialityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (speciality, error) = await _adminSpecialityService.CreateSpecialityAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetSpeciality), new { id = speciality!.Id }, speciality);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpeciality(Guid id, [FromBody] UpdateSpecialityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminSpecialityService.UpdateSpecialityAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpeciality(Guid id)
    {
        var (success, error) = await _adminSpecialityService.DeleteSpecialityAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
