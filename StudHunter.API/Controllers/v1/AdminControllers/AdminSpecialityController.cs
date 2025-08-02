using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing specialties with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminSpecialityController(AdminSpecialityService adminSpecialityService) : BaseController
{
    private readonly AdminSpecialityService _adminSpecialityService = adminSpecialityService;

    /// <summary>
    /// Retrieves all specialties.
    /// </summary>
    /// <returns>A list of all specialties.</returns>
    /// <response code="200">Specialties retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SpecialityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllSpecialties()
    {
        var (specialties, statusCode, errorMessage) = await _adminSpecialityService.GetAllSpecialtiesAsync();
        return CreateAPIError(specialties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a specialty by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the specialty.</param>
    /// <returns>The specialty.</returns>
    /// <response code="200">Specialty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Specialty not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpecialityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpeciality(Guid id)
    {
        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.GetSpecialityAsync(id);
        return CreateAPIError(speciality, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new specialty.
    /// </summary>
    /// <param name="dto">The data transfer object containing specialty details.</param>
    /// <returns>The created specialty.</returns>
    /// <response code="201">Specialty created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="409">A specialty with the specified name already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SpecialityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSpeciality([FromBody] CreateSpecialityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.CreateSpecialityAsync(dto);
        return CreateAPIError(speciality, statusCode, errorMessage, nameof(GetSpeciality), new { id = speciality?.Id });
    }

    /// <summary>
    /// Updates an existing specialty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the specialty.</param>
    /// <param name="dto">The data transfer object containing updated specialty details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Specialty updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Specialty not found.</response>
    /// <response code="409">A specialty with the specified name already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateSpeciality(Guid id, [FromBody] UpdateSpecialityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (success, statusCode, errorMessage) = await _adminSpecialityService.UpdateSpecialityAsync(id, dto);
        return CreateAPIError<SpecialityDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a specialty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the specialty.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Specialty deleted successfully.</response>
    /// <response code="400">Invalid request data or specialty is associated with study plans.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Specialty not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSpeciality(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminSpecialityService.DeleteSpecialityAsync(id);
        return CreateAPIError<SpecialityDto>(success, statusCode, errorMessage);
    }
}
