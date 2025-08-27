using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Auth;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services;
using System.ComponentModel.DataAnnotations;
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
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <returns>The employer details.</returns>
    /// <response code="200">Employer retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this employer.</response>
    /// <response code="404">Employer not found.</response>
    [HttpGet("{employerId}")]
    [ProducesResponseType(typeof(EmployerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployer(Guid employerId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<EmployerDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (employer, statusCode, errorMessage) = await _employerService.GetEmployerAsync(employerId, authUserId);
        return HandleResponse(employer, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an employer by their email.
    /// </summary>
    /// <param name="email">The email address of the employer.</param>
    /// <returns>The employer details.</returns>
    /// <response code="200">Employer retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this employer.</response>
    /// <response code="404">Employer not found.</response>
    [HttpGet("employer/{email}")]
    [ProducesResponseType(typeof(EmployerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployer(string email)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<EmployerDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (employer, statusCode, errorMessage) = await _employerService.GetEmployerAsync(email, authUserId);
        return HandleResponse(employer, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves employers by specialization.
    /// </summary>
    /// <param name="specialization">The specialization to filter employers (optional).</param>
    /// <returns>A list of employers.</returns>
    /// <response code="200">Employers retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a student or accredited employer.</response>
    [HttpGet("specialization")]
    [ProducesResponseType(typeof(List<EmployerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEmployerBySpecialization([FromQuery, StringLength(255)] string? specialization)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<EmployerDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (employers, statusCode, errorMessage) = await _employerService.GetEmployersBySpecializationAsync(specialization, authUserId);
        return HandleResponse(employers, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new employer.
    /// </summary>
    /// <param name="dto">The data transfer object containing employer details.</param>
    /// <returns>The created employer.</returns>
    /// <response code="201">Employer created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="409">Email already exists.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EmployerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEmployer([FromBody] RegisterEmployerDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<EmployerDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (employer, statusCode, errorMessage) = await _employerService.CreateEmployerAsync(dto);
        return HandleResponse(employer, statusCode, errorMessage, nameof(GetEmployer), new { employerId = employer?.Id });
    }

    /// <summary>
    /// Updates an existing employer.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Employer updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to update this employer.</response>
    /// <response code="404">Employer not found.</response>
    /// <response code="409">Email or phone number already exists.</response>
    /// <response code="410">Employer has been deleted.</response>
    [HttpPut("{employerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateEmployer(Guid employerId, [FromBody] UpdateEmployerDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _employerService.UpdateEmployerAsync(authUserId, employerId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an employer (soft delete).
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Employer deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to delete this employer.</response>
    /// <response code="404">Employer not found.</response>
    /// <response code="410">Employer has already been deleted.</response>
    [HttpDelete("{employerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteEmployer(Guid employerId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _employerService.DeleteEmployerAsync(authUserId, employerId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
