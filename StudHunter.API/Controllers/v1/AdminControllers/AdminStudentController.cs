using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.StudentDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing students with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminStudentController(AdminStudentService adminStudentService) : BaseController
{
    private readonly AdminStudentService _adminStudentService = adminStudentService;

    /// <summary>
    /// Retrieves all students.
    /// </summary>
    /// <returns>A list of all students.</returns>
    /// <response code="200">Students retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AdminStudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllStudents()
    {
        var (students, statusCode, errorMessage) = await _adminStudentService.GetAllStudentsAsync();
        return HandleResponse(students, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a student by their ID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>The student.</returns>
    /// <response code="200">Student retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Student not found.</response>
    [HttpGet("{studentId}")]
    [ProducesResponseType(typeof(AdminStudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(Guid studentId)
    {
        var (student, statusCode, errorMessage) = await _adminStudentService.GetStudentAsync(studentId);
        return HandleResponse(student, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a student by their email.
    /// </summary>
    /// <param name="email">The email of the student.</param>
    /// <returns>The student.</returns>
    /// <response code="200">Student retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Student not found.</response>
    [HttpGet("student/{email}")]
    [ProducesResponseType(typeof(AdminStudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(string email)
    {
        var (student, statusCode, errorMessage) = await _adminStudentService.GetStudentAsync(email);
        return HandleResponse(student, statusCode, errorMessage);
    }

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Student updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Student, faculty, or speciality not found.</response>
    /// <response code="409">A student with the specified email already exists.</response>
    [HttpPut("{studentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudent(Guid studentId, [FromBody] AdminUpdateStudentDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminStudentService.UpdateStudentAsync(studentId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a student (hard or soft delete).
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Student deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Student not found.</response>
    [HttpDelete("{studentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(Guid studentId, [FromQuery] bool hardDelete = false)
    {
        var (success, statusCode, errorMessage) = await _adminStudentService.DeleteStudentAsync(studentId, hardDelete);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
