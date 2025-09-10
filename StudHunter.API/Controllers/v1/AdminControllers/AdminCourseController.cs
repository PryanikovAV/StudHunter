using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.CourseDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing courses with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminCourseController(AdminCourseService adminCourseService) : BaseController
{
    private readonly AdminCourseService _adminCourseService = adminCourseService;

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
        var (courses, statusCode, errorMessage) = await _adminCourseService.GetAllCoursesAsync();
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
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(Guid courseId)
    {
        var (course, statusCode, errorMessage) = await _adminCourseService.GetCourseAsync(courseId);
        return HandleResponse(course, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="dto">The data transfer object containing course details.</param>
    /// <returns>The created course.</returns>
    /// <response code="201">Course created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="409">A course with the specified name already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<CourseDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (course, statusCode, errorMessage) = await _adminCourseService.CreateCourseAsync(dto);
        return HandleResponse(course, statusCode, errorMessage, nameof(GetCourse), new { courseId = course?.Id });
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <param name="dto">The data transfer object containing updated course details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Course updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Course not found.</response>
    /// <response code="409">A course with the specified name already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] UpdateCourseDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminCourseService.UpdateCourseAsync(courseId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a course.
    /// </summary>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Course deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Course not found.</response>
    [HttpDelete("{courseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(Guid courseId)
    {
        var (success, statusCode, errorMessage) = await _adminCourseService.DeleteCourseAsync(courseId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
