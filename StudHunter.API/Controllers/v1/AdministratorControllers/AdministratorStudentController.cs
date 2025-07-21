using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdministratorStudentController(AdministratorStudentService administratorStudentService) : ControllerBase
{
    private readonly AdministratorStudentService _administratorStudentService = administratorStudentService;

    /// <summary>
    /// Retrieves a list of all students.
    /// </summary>
    /// <returns>A list of all students' details.</returns>
    /// <response code="200">Returns the list of all students.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    /// <response code="403">Forbidden if the user is not an administrator.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudents()
    {
        var students = await _administratorStudentService.GetAllStudentsAsync();
        return Ok(students);
    }

    /// <summary>
    /// Retrieves a student by their unique ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>The student's details if found; otherwise, a 404 error.</returns>
    /// <response code="200">Returns the student's details.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    /// <response code="403">Forbidden if the user is not an administrator.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetStudent(Guid id)
    {
        var student = await _administratorStudentService.GetStudentAsync(id);
        if (student == null)
            return NotFound();
        return Ok(student);
    }

    /// <summary>
    /// Updates an existing student's data as an administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student to update.</param>
    /// <param name="dto">The updated student data.</param>
    /// <returns>No content if successful; otherwise, an error.</returns>
    /// <response code="204">Student was updated successfully.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    /// <response code="403">Forbidden if the user is not an administrator.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    /// <response code="409">Conflict due to duplicate email or invalid data.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentByAdministratorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _administratorStudentService.UpdateStudentAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    /// <summary>
    /// Deletes a student by their unique ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student to delete.</param>
    /// <returns>No content if successful; otherwise, an error.</returns>
    /// <response code="204">Student was deleted successfully.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    /// <response code="403">Forbidden if the user is not an administrator.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    /// <response code="409">Conflict due to dependencies or other issues.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var (success, error) = await _administratorStudentService.DeleteStudentAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
