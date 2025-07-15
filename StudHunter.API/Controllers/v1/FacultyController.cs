using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class FacultyController(FacultyService facultyService) : ControllerBase
{
    private readonly FacultyService _facultyService = facultyService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var course = await _facultyService.GetFacultyAsync(id);
        if (course == null)
            return NotFound();
        return Ok(course);
    }
}