using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminSpecialityController(AdminSpecialityService adminSpecialityService) : BaseController
{
    private readonly AdminSpecialityService _adminSpecialityService = adminSpecialityService;

    [HttpGet]
    public async Task<IActionResult> GetAllSpecialties()
    {
        var (specialties, statusCode, errorMessage) = await _adminSpecialityService.GetAllSpecialtiesAsync();
        return this.CreateAPIError(specialties, statusCode, errorMessage);
    }

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
        if (speciality == null)
            return error == null ? NotFound() : BadRequest(new { error });
        return CreatedAtAction(nameof(GetSpeciality), new { id = speciality!.Id }, speciality);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpeciality(Guid id, [FromBody] UpdateSpecialityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminSpecialityService.UpdateSpecialityAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpeciality(Guid id)
    {
        var (success, error) = await _adminSpecialityService.DeleteSpecialityAsync(id);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
