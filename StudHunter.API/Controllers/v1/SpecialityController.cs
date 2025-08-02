using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing specialties.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class SpecialityController(SpecialityService specialityService) : BaseController
{
    private readonly SpecialityService _specialityService = specialityService;

    /// <summary>
    /// Retrieves all specialties.
    /// </summary>
    /// <returns>A list of all specialties.</returns>
    /// <response code="200">Specialties retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SpecialityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllSpecialties()
    {
        var (specialties, statusCode, errorMessage) = await _specialityService.GetAllSpecialtiesAsync();
        return CreateAPIError(specialties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a specialty by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the specialty.</param>
    /// <returns>The specialty.</returns>
    /// <response code="200">Specialty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Specialty not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpecialityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpeciality(Guid id)
    {
        var (speciality, statusCode, errorMessage) = await _specialityService.GetSpecialityAsync(id);
        return CreateAPIError(speciality, statusCode, errorMessage);
    }
}
