using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class StudentController(StudentService studentService) : BaseController
{
    private readonly StudentService _studentService = studentService;

    /// <summary>
    /// Retrieves a student by their unique ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>The student's details if found; otherwise, a 404 error.</returns>
    /// <response code="200">Returns the student's details.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var (student, statusCode, errorMessage) = await _studentService.GetStudentAsync(id);
        return this.CreateAPIError(student, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="dto">The student data to create.</param>int? statusCode)
    /// <returns>The created student's details with a 201 status.</returns>
    /// <response code="201">Student was created successfully.</response>
    /// <response code="400">Invalid student data provided.</response>
    /// <response code="409">Student with the specified email already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (student, statusCode, errorMessage) = await _studentService.CreateStudentAsync(dto);
        return this.CreateAPIError(student, statusCode, errorMessage, nameof(GetStudent), new { id = student?.Id });
    }

    /// <summary>
    /// Updates an existing student's data.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student to update.</param>
    /// <param name="dto">The updated student data.</param>
    /// <returns>No content if successful; otherwise, an error.</returns>
    /// <response code="204">Student was updated successfully.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    /// <response code="409">Conflict due to duplicate email or invalid data.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (success, statusCode, errorMessage) = await _studentService.UpdateStudentAsync(id, dto);
        return this.CreateAPIError<StudentDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Soft deletes a student.
    /// </summary>
    /// <param name="id">The unique identifier of the student to delete.</param>
    /// <returns>No content if successful; otherwise, an error.</returns>
    /// <response code="204">Student was soft deleted successfully.</response>
    /// <response code="404">Student not found.</response>
    /// <response code="400">Invalid operation, e.g., student already deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (success, statusCode, errorMessage) = await _studentService.DeleteStudentAsync(id);
        return this.CreateAPIError<StudentDto>(success, statusCode, errorMessage);
    }
}
