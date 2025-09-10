using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.SpecialityDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing specialties with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
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
        return HandleResponse(specialties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a specialty by its ID.
    /// </summary>
    /// <param name="specialityId">The unique identifier (GUID) of the specialty.</param>
    /// <returns>The specialty.</returns>
    /// <response code="200">Specialty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Specialty not found.</response>
    [HttpGet("{specialityId}")]
    [ProducesResponseType(typeof(SpecialityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpeciality(Guid specialityId)
    {
        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.GetSpecialityAsync(specialityId);
        return HandleResponse(speciality, statusCode, errorMessage);
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
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<SpecialityDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.CreateSpecialityAsync(dto);
        return HandleResponse(speciality, statusCode, errorMessage, nameof(GetSpeciality), new { specialityId = speciality?.Id });
    }

    /// <summary>
    /// Updates an existing specialty.
    /// </summary>
    /// <param name="specialityId">The unique identifier (GUID) of the specialty.</param>
    /// <param name="dto">The data transfer object containing updated specialty details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Specialty updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Specialty not found.</response>
    /// <response code="409">A specialty with the specified name already exists.</response>
    [HttpPut("{specialityId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateSpeciality(Guid specialityId, [FromBody] UpdateSpecialityDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminSpecialityService.UpdateSpecialityAsync(specialityId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a specialty.
    /// </summary>
    /// <param name="specialityId">The unique identifier (GUID) of the specialty.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Specialty deleted successfully.</response>
    /// <response code="400">Invalid request data or specialty is associated with study plans.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Specialty not found.</response>
    [HttpDelete("{specialityId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSpeciality(Guid specialityId)
    {
        var (success, statusCode, errorMessage) = await _adminSpecialityService.DeleteSpecialityAsync(specialityId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
