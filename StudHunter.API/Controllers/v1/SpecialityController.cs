using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.SpecialityDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;

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
        return HandleResponse(specialties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a specialty by its ID.
    /// </summary>
    /// <param name="specialityId">The unique identifier (GUID) of the specialty.</param>
    /// <returns>The specialty.</returns>
    /// <response code="200">Specialty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Specialty not found.</response>
    [HttpGet("{specialityId}")]
    [ProducesResponseType(typeof(SpecialityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpeciality(Guid specialityId)
    {
        var (speciality, statusCode, errorMessage) = await _specialityService.GetSpecialityAsync(specialityId);
        return HandleResponse(speciality, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a speciality by its name.
    /// </summary>
    /// <param name="specialityName"></param>
    /// <returns>The specialty.</returns>
    /// <response code="200">Specialty retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Specialty not found.</response>
    [HttpGet("speciality/{specialityName}")]
    [ProducesResponseType(typeof(SpecialityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpeciality(string specialityName)
    {
        var (speciality, statusCode, errorMessage) = await _specialityService.GetSpecialityAsync(specialityName);
        return HandleResponse(speciality, statusCode, errorMessage);
    }
}
