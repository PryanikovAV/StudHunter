using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing students.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class StudentController(StudentService studentService) : BaseController
{
    private readonly StudentService _studentService = studentService;

    /// <summary>
    /// Retrieves a student by their ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>The student.</returns>
    /// <response code="200">Student retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Student or study plan not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var (student, statusCode, errorMessage) = await _studentService.GetStudentAsync(id);
        return CreateAPIError(student, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="dto">The data transfer object containing student details.</param>
    /// <returns>The created student.</returns>
    /// <response code="201">Student created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Faculty or speciality not found.</response>
    /// <response code="409">A student with the specified email already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (student, statusCode, errorMessage) = await _studentService.CreateStudentAsync(dto);
        return CreateAPIError(student, statusCode, errorMessage, nameof(GetStudent), new { id = student?.Id });
    }

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Student updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Student, faculty, or speciality not found.</response>
    /// <response code="409">A student with the specified email already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (success, statusCode, errorMessage) = await _studentService.UpdateStudentAsync(id, dto);
        return CreateAPIError<StudentDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a student (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Student deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Student not found.</response>
    /// <response code="410">Student has been deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var (success, statusCode, errorMessage) = await _studentService.DeleteStudentAsync(id);
        return CreateAPIError<StudentDto>(success, statusCode, errorMessage);
    }
}
