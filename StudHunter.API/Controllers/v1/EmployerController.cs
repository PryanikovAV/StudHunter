using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Auth;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing employers.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class EmployerController(EmployerService employerService) : BaseController
{
    private readonly EmployerService _employerService = employerService;

    /// <summary>
    /// Retrieves an employer by their ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <returns>The employer.</returns>
    /// <response code="200">Employer retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Employer not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployer(Guid id)
    {
        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<EmployerDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (userId != id && User.IsInRole(nameof(Administrator)))
            return CreateAPIError<EmployerDto>(null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", "profile"));

        var (employer, statusCode, errorMessage) = await _employerService.GetEmployerAsync<EmployerDto>(id);
        return CreateAPIError(employer, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an employer by their specialization.
    /// </summary>
    /// <param name="specialization">Employer specialization.</param>
    /// <returns>The employer.</returns>
    /// <response code="200">Employer retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("specialization")]
    [ProducesResponseType(typeof(EmployerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEmployerBySpecialization([FromQuery] string? specialization)
    {
        var (employers, statusCode, errorMessage) = await _employerService.GetEmployersBySpecializationAsync<EmployerDto>(specialization);
        return CreateAPIError(employers, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new employer.
    /// </summary>
    /// <param name="dto">The data transfer object containing employer details.</param>
    /// <returns>The created employer.</returns>
    /// <response code="201">Employer created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="409">An employer with the specified email already exists.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EmployerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEmployer([FromBody] RegisterEmployerDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (employer, statusCode, errorMessage) = await _employerService.CreateEmployerAsync(dto);
        return CreateAPIError(employer, statusCode, errorMessage, nameof(GetEmployer), new { id = employer?.Id });
    }

    /// <summary>
    /// Updates an existing employer.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Employer updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403"></response>
    /// <response code="404">Employer not found.</response>
    /// <response code="409">An employer with the specified email already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] UpdateEmployerDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<EmployerDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (userId != id)
            return CreateAPIError<EmployerDto>(null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", "profile"));

        var (success, statusCode, errorMessage) = await _employerService.UpdateEmployerAsync(id, dto);
        return CreateAPIError<EmployerDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an employer.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Employer deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403"></response>
    /// <response code="404">Employer not found.</response>
    /// <response code="410">Employer is already deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteEmployer(Guid id)
    {
        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<EmployerDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (userId != id)
            return CreateAPIError<EmployerDto>(null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("delete", "profile"));

        var (success, statusCode, errorMessage) = await _employerService.DeleteEmployerAsync(id);
        return CreateAPIError<EmployerDto>(success, statusCode, errorMessage);
    }
}
