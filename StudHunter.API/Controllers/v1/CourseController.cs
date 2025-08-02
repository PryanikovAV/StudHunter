using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing courses.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class CourseController(CourseService courseService) : BaseController
{
    private readonly CourseService _courseService = courseService;

    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    /// <returns>A list of courses.</returns>
    /// <response code="200">Courses retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllCourses()
    {
        var (courses, statusCode, errorMessage) = await _courseService.GetAllCoursesAsync();
        return CreateAPIError(courses, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a course by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the course.</param>
    /// <returns>The course.</returns>
    /// <response code="200">Course retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Course not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var (course, statusCode, errorMessage) = await _courseService.GetCourseAsync(id);
        return CreateAPIError(course, statusCode, errorMessage);
    }
}
