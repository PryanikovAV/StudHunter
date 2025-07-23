using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminStudentController(AdminStudentService adminStudentService) : ApiControllerBase
{
    private readonly AdminStudentService _adminStudentService = adminStudentService;
    // TODO: add new documentation
    [HttpGet]
    [ProducesResponseType(typeof(List<StudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStudents()
    {
        var (students, statusCode, errorMessage) = await _adminStudentService.GetAllStudentsAsync();
        return this.CreateAPIError(students, statusCode, errorMessage);
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
        var student = await _adminStudentService.GetStudentAsync(id);
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

        var (success, error) = await _adminStudentService.UpdateStudentAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    /// <summary>
    /// Deletes a student by their unique ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student to delete.</param>
    /// <param name="hardDelete">Query parameter indicating the deletion option: hard or soft deletion.</param>
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
    public async Task<IActionResult> DeleteStudent(Guid id, [FromQuery] bool hardDelete = false)
    {
        var (success, error) = await _adminStudentService.DeleteStudentAsync(id, hardDelete);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
