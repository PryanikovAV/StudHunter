using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing vacancies with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminVacancyController(AdminVacancyService adminVacancyService) : BaseController
{
    private readonly AdminVacancyService _adminVacancyService = adminVacancyService;

    /// <summary>
    /// Retrieves all vacancies, including deleted ones.
    /// </summary>
    /// <returns>A list of all vacancies.</returns>
    /// <response code="200">Vacancies retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<VacancyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllVacancies()
    {
        var (vacancies, statusCode, errorMessage) = await _adminVacancyService.GetAllVacanciesAsync();
        return CreateAPIError(vacancies, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a vacancy by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <returns>The vacancy.</returns>
    /// <response code="200">Vacancy retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Vacancy not found.</response>
    /// <response code="410">Vacancy has been deleted.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> GetVacancy(Guid id)
    {
        var (vacancy, statusCode, errorMessage) = await _adminVacancyService.GetVacancyAsync(id);
        return CreateAPIError(vacancy, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves all vacancies for a specific employer.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <returns>A list of vacancies.</returns>
    /// <response code="200">Vacancies retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Employer not found.</response>
    [HttpGet("by-employer/{id}")]
    [ProducesResponseType(typeof(List<VacancyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVacanciesByEmployer(Guid id)
    {
        var (vacancies, statusCode, errorMessage) = await _adminVacancyService.GetVacanciesByEmployerAsync(id);
        return CreateAPIError(vacancies, statusCode, errorMessage);
    }

    /// <summary>
    /// Updates an existing vacancy.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="dto">The data transfer object containing updated vacancy details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Vacancy updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Vacancy not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] AdminUpdateVacancyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (success, statusCode, errorMessage) = await _adminVacancyService.UpdateVacancyAsync(id, dto);
        return CreateAPIError<VacancyDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Associates a course with a vacancy.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Course associated with vacancy successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Vacancy or course not found.</response>
    /// <response code="409">Course already associated with vacancy.</response>
    [HttpPost("{id}/courses")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCourseToVacancy(Guid vacancyId, [FromBody] Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (success, statusCode, errorMessage) = await _adminVacancyService.AddCourseToVacancyAsync(vacancyId, courseId);
        return CreateAPIError<VacancyDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Removes a course association from a vacancy.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Course association removed successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Course not associated with vacancy.</response>
    [HttpDelete("{id}/courses/{courseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCourseFromVacancy(Guid vacancyId, Guid courseId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (success, statusCode, errorMessage) = await _adminVacancyService.RemoveCourseFromVacancyAsync(vacancyId, courseId);
        return CreateAPIError<VacancyDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a vacancy (hard or soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Vacancy deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Vacancy not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVacancy(Guid id, [FromQuery] bool hardDelete = false)
    {
        var (success, statusCode, errorMessage) = await _adminVacancyService.DeleteVacancyAsync(id, hardDelete);
        return CreateAPIError<VacancyDto>(success, statusCode, errorMessage);
    }
}
