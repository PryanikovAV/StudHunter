using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class CourseController(CourseService courseService) : ControllerBase
{
    private readonly CourseService _courseService = courseService;

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _courseService.GetCoursesAsync(); 
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var course = await _courseService.GetCourseAsync(id);
        if (course == null)
            return NotFound();
        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var (course, error) = await _courseService.CreateCourseAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetCourse), new { id = course!.Id }, course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto dto)
    {
        var (success, error) = await _courseService.UpdateCourseAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var (success, error) = await _courseService.DeleteCourseAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
