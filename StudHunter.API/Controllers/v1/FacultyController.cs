using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class FacultyController(FacultyService facultyService) : ControllerBase
{
    private readonly FacultyService _facultyService = facultyService;

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _facultyService.GetFacultiesAsync();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var course = await _facultyService.GetFacultyAsync(id);
        if (course == null)
            return NotFound();
        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateFacultyDto dto)
    {
        var (faculty, error) = await _facultyService.CreateFacultyAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetCourse), new { id = faculty!.Id }, faculty);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateFacultyDto dto)
    {
        var (success, error) = await _facultyService.UpdateFacultyAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var (success, error) = await _facultyService.DeleteFacultyAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }