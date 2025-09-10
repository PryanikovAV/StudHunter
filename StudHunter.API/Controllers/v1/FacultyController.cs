using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.FacultyDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing faculties.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class FacultyController(FacultyService facultyService) : BaseController
{
    private readonly FacultyService _facultyService = facultyService;

    /// <summary>
    /// Retrieves all faculties.
    /// </summary>
    /// <returns>A list of faculties.</returns>
    /// <response code="200">Faculties retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FacultyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFaculties()
    {
        var (faculties, statusCode, errorMessage) = await _facultyService.GetAllFacultiesAsync();
        return HandleResponse(faculties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a faculty by its ID.
    /// </summary>
    /// <param name="facultyId">The unique identifier (GUID) of the faculty.</param>
    /// <returns>The faculty.</returns>
    /// <response code="200">Faculty retrieved successfully.</response>
    /// <response code="404">Faculty not found.</response>
    [HttpGet("{facultyId}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaculty(Guid facultyId)
    {
        var (faculty, statusCode, errorMessage) = await _facultyService.GetFacultyAsync(facultyId);
        return HandleResponse(faculty, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a faculty by its name.
    /// </summary>
    /// <param name="facultyName"></param>
    /// <returns></returns>
    [HttpGet("faculty{facultyName}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaculty(string facultyName)
    {
        var (faculty, statusCode, errorMessage) = await _facultyService.GetFacultyAsync(facultyName);
        return HandleResponse(faculty, statusCode, errorMessage);
    }
}
