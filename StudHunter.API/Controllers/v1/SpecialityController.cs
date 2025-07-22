using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class SpecialityController(SpecialityService specialityService) : ControllerBase
{
    private readonly SpecialityService _specialityService = specialityService;

    [HttpGet]
    public async Task<IActionResult> GetSpecialities()
    {
        var specialities = await _specialityService.GetAllSpecialitiesAsync();
        return Ok(specialities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSpeciality(Guid id)
    {
        var speciality = await _specialityService.GetSpecialityAsync(id);
        if (speciality == null)
            return NotFound();
        return Ok(speciality);
    }
}
