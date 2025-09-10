using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.AuthDto;
using StudHunter.API.ModelsDto.StudentDto;
using StudHunter.API.Services;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class StudentController(StudentService studentService) : BaseController
{
    private readonly StudentService _studentService = studentService;

    /// <summary>
    /// Retrieves a student by their ID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>The student details.</returns>
    /// <response code="200">Student retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this student.</response>
    /// <response code="404">Student not found.</response>
    [HttpGet("{studentId}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(Guid studentId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (student, statusCode, errorMessage) = await _studentService.GetStudentAsync(authUserId, studentId);
        return HandleResponse(student, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a student by their email.
    /// </summary>
    /// <param name="email">The email address of the student.</param>
    /// <returns>The student details.</returns>
    /// <response code="200">Student retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this student.</response>
    /// <response code="404">Student not found.</response>
    [HttpGet("student/{email}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(string email)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (student, statusCode, errorMessage) = await _studentService.GetStudentAsync(authUserId, email);
        return HandleResponse(student, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="dto">The data transfer object containing student details.</param>
    /// <returns>The created student.</returns>
    /// <response code="201">Student created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="409">Email already exists.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] RegisterStudentDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<StudentDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (student, statusCode, errorMessage) = await _studentService.CreateStudentAsync(dto);
        return HandleResponse(student, statusCode, errorMessage, nameof(GetStudent), new { studentId = student?.Id });
    }

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Student updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to update this student.</response>
    /// <response code="404">Student not found.</response>
    /// <response code="409">Email or phone number already exists.</response>
    /// <response code="410">Student has been deleted.</response>
    [HttpPut("{studentId}")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateStudent(Guid studentId, [FromBody] UpdateStudentDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _studentService.UpdateStudentAsync(authUserId, studentId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a student (soft delete).
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Student deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to delete this student.</response>
    /// <response code="404">Student not found.</response>
    /// <response code="410">Student has already been deleted.</response>
    [HttpDelete("{studentId}")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteStudent(Guid studentId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _studentService.DeleteStudentAsync(authUserId, studentId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
