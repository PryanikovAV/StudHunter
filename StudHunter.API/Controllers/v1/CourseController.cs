using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.CourseDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;

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
        return HandleResponse(courses, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a course by its ID.
    /// </summary>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>The course.</returns>
    /// <response code="200">Course retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Course not found.</response>
    [HttpGet("{courseId}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(Guid courseId)
    {
        var (course, statusCode, errorMessage) = await _courseService.GetCourseAsync(courseId);
        return HandleResponse(course, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a course by its name.
    /// </summary>
    /// <param name="courseName"></param>
    /// <returns>The course.</returns>
    /// <response code="200">Course retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Course not found.</response>
    [HttpGet("course/{courseName}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(string courseName)
    {
        var (course, statusCode, errorMessage) = await _courseService.GetCourseAsync(courseName);
        return HandleResponse(course, statusCode, errorMessage);
    }
}
