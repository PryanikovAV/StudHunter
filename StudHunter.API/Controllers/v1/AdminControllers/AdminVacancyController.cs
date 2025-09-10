using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.VacancyDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;
using System.Data;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing vacancies with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
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
        return HandleResponse(vacancies, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a vacancy by its ID.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <returns>The vacancy.</returns>
    /// <response code="200">Vacancy retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Vacancy not found.</response>
    /// <response code="410">Vacancy has been deleted.</response>
    [HttpGet("{vacancyId}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> GetVacancy(Guid vacancyId)
    {
        var (vacancy, statusCode, errorMessage) = await _adminVacancyService.GetVacancyAsync(vacancyId);
        return HandleResponse(vacancy, statusCode, errorMessage);
    }

    /// <summary>
    /// Updates an existing vacancy.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="dto">The data transfer object containing updated vacancy details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Vacancy updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Vacancy not found.</response>
    [HttpPut("{vacancyId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVacancy(Guid vacancyId, [FromBody] UpdateVacancyDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminVacancyService.UpdateVacancyAsync(vacancyId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    [HttpPut("{vacancyId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVacancyStatus(Guid vacancyId, [FromBody] UpdateStatusDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminVacancyService.UpdateVacancyStatusAsync(vacancyId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a vacancy (hard delete).
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Vacancy deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Vacancy not found.</response>
    [HttpDelete("{vacancyId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVacancy(Guid vacancyId)
    {
        var (success, statusCode, errorMessage) = await _adminVacancyService.DeleteVacancyAsync(vacancyId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
