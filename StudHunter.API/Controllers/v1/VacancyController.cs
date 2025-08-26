using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services;
using System.Data;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing vacancies.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class VacancyController(VacancyService vacancyService) : BaseController
{
    private readonly VacancyService _vacancyService = vacancyService;

    /// <summary>
    /// Retrieves all non-deleted vacancies.
    /// </summary>
    /// <returns>A list of vacancies.</returns>
    /// <response code="200">Vacancies retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<VacancyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllVacancies()
    {
        var (vacancies, statusCode, errorMessage) = await _vacancyService.GetAllVacanciesAsync();
        return HandleResponse(vacancies, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a vacancy by its ID.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <returns>The vacancy.</returns>
    /// <response code="200">Vacancy retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Vacancy not found.</response>
    /// <response code="410">Vacancy has been deleted.</response>
    [HttpGet("{vacancyId}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVacancy(Guid vacancyId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<VacancyDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (vacancy, statusCode, errorMessage) = await _vacancyService.GetVacancyAsync(vacancyId, authUserId);
        return HandleResponse(vacancy, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves all vacancies for a specific employer.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <returns>A list of vacancies.</returns>
    /// <response code="200">Vacancies retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Employer not found.</response>
    [HttpGet("employer/{employerId}/vacancies")]
    [ProducesResponseType(typeof(List<VacancyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVacanciesByEmployer(Guid employerId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<VacancyDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (vacancies, statusCode, errorMessage) = await _vacancyService.GetVacanciesByEmployerAsync(employerId, authUserId);
        return HandleResponse(vacancies, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new vacancy.
    /// </summary>
    /// <param name="dto">The data transfer object containing vacancy details.</param>
    /// <returns>The created vacancy.</returns>
    /// <response code="201">Vacancy created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Employer is not accredited.</response>
    /// <response code="404">Employer not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancyDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<VacancyDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<VacancyDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (vacancy, statusCode, errorMessage) = await _vacancyService.CreateVacancyAsync(authUserId, dto);
        return HandleResponse(vacancy, statusCode, errorMessage, nameof(GetVacancy), new { vacancyId = vacancy?.Id });
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
    /// <response code="404">Vacancy not found.</response>
    /// <response code="410">Vacancy has been deleted.</response>
    [HttpPut("{vacancyId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateVacancy(Guid vacancyId, [FromBody] UpdateVacancyDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _vacancyService.UpdateVacancyAsync(authUserId, vacancyId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    [HttpPut("{vacancyId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateVacancyStatus(Guid vacancyId, [FromBody] UpdateStatusDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _vacancyService.UpdateVacancyStatusAsync(authUserId, vacancyId, dto.IsDeleted);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
