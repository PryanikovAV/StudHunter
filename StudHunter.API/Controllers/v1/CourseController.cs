using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class CourseController(CourseService courseService) : BaseController
{
    private readonly CourseService _courseService = courseService;

    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        var (courses, statusCode, errorMessage) = await _courseService.GetAllCoursesAsync();
        return this.CreateAPIError(courses, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var (course, statusCode, errorMessage) = await _courseService.GetCourseAsync(id);
        return this.CreateAPIError(course, statusCode, errorMessage);
    }
}
