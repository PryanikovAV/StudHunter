using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing faculties with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminFacultyController(AdminFacultyService adminFacultyService) : BaseController
{
    private readonly AdminFacultyService _adminFacultyService = adminFacultyService;

    /// <summary>
    /// Retrieves all faculties.
    /// </summary>
    /// <returns>A list of faculties.</returns>
    /// <response code="200">Faculties retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FacultyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllFaculties()
    {
        var (faculties, statusCode, errorMessage) = await _adminFacultyService.GetAllFacultiesAsync();
        return CreateAPIError(faculties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a faculty by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the faculty.</param>
    /// <returns>The faculty.</returns>
    /// <response code="200">Faculty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Faculty not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var (faculty, statusCode, errorMessage) = await _adminFacultyService.GetFacultyAsync(id);
        return CreateAPIError(faculty, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new faculty.
    /// </summary>
    /// <param name="dto">The data transfer object containing faculty details.</param>
    /// <returns>The created faculty.</returns>
    /// <response code="201">Faculty created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="409">A faculty with the specified name already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (faculty, statusCode, errorMessage) = await _adminFacultyService.CreateFacultyAsync(dto);
        return CreateAPIError(faculty, statusCode, errorMessage, nameof(GetFaculty), new { id = faculty?.Id });
    }

    /// <summary>
    /// Updates an existing faculty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the faculty.</param>
    /// <param name="dto">The data transfer object containing updated faculty details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Faculty updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Faculty not found.</response>
    /// <response code="409">A faculty with the specified name already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateFaculty(Guid id, [FromBody] UpdateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (success, statusCode, errorMessage) = await _adminFacultyService.UpdateFacultyAsync(id, dto);
        return CreateAPIError<FacultyDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a faculty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the faculty.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Faculty deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Faculty not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFaculty(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminFacultyService.DeleteFacultyAsync(id);
        return CreateAPIError<FacultyDto>(success, statusCode, errorMessage);
    }
}
