using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class FacultyController(FacultyService facultyService) : ControllerBase
{
    private readonly FacultyService _facultyService = facultyService;

    [HttpGet]
    public async Task<IActionResult> GetAllFaculties()
    {
        var faculty = await _facultyService.GetAllFacultiesAsync();
        return Ok(faculty);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var faculty = await _facultyService.GetFacultyAsync(id);
        if (faculty == null)
            return NotFound();
        return Ok(faculty);
    }
}
