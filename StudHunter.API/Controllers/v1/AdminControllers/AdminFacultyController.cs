using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.FacultyDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing faculties with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
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
        return HandleResponse(faculties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a faculty by its ID.
    /// </summary>
    /// <param name="facultyId">The unique identifier (GUID) of the faculty.</param>
    /// <returns>The faculty.</returns>
    /// <response code="200">Faculty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Faculty not found.</response>
    [HttpGet("{facultyId}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaculty(Guid facultyId)
    {
        var (faculty, statusCode, errorMessage) = await _adminFacultyService.GetFacultyAsync(facultyId);
        return HandleResponse(faculty, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a faculty by its name.
    /// </summary>
    /// <param name="name">The name of the faculty.</param>
    /// <returns>The faculty.</returns>
    /// <response code="200">Faculty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Faculty not found.</response>
    [HttpGet("faculty/{name}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaculty(string name)
    {
        var (faculty, statusCode, errorMessage) = await _adminFacultyService.GetFacultyAsync(name);
        return HandleResponse(faculty, statusCode, errorMessage);
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
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<FacultyDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (faculty, statusCode, errorMessage) = await _adminFacultyService.CreateFacultyAsync(dto);
        return HandleResponse(faculty, statusCode, errorMessage, nameof(GetFaculty), new { facultyId = faculty?.Id });
    }

    /// <summary>
    /// Updates an existing faculty.
    /// </summary>
    /// <param name="facultyId">The unique identifier (GUID) of the faculty.</param>
    /// <param name="dto">The data transfer object containing updated faculty details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Faculty updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Faculty not found.</response>
    /// <response code="409">A faculty with the specified name already exists.</response>
    [HttpPut("{facultyId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateFaculty(Guid facultyId, [FromBody] UpdateFacultyDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminFacultyService.UpdateFacultyAsync(facultyId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a faculty.
    /// </summary>
    /// <param name="facultyId">The unique identifier (GUID) of the faculty.</param>
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
    public async Task<IActionResult> DeleteFaculty(Guid facultyId)
    {
        var (success, statusCode, errorMessage) = await _adminFacultyService.DeleteFacultyAsync(facultyId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
