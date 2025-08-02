using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing faculties.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
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
        return CreateAPIError(faculties, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a faculty by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the faculty.</param>
    /// <returns>The faculty.</returns>
    /// <response code="200">Faculty retrieved successfully.</response>
    /// <response code="404">Faculty not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var (faculty, statusCode, errorMessage) = await _facultyService.GetFacultyAsync(id);
        return CreateAPIError(faculty, statusCode, errorMessage);
    }
}
