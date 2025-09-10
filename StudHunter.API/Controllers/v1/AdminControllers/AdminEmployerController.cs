using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.EmployerDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing employers with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminEmployerController(AdminEmployerService adminEmployerService) : BaseController
{
    private readonly AdminEmployerService _adminEmployerService = adminEmployerService;

    /// <summary>
    /// Retrieves all employers.
    /// </summary>
    /// <returns>A list of employers.</returns>
    /// <response code="200">Employers retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AdminEmployerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllEmployers()
    {
        var (employers, statusCode, errorMessage) = await _adminEmployerService.GetAllEmployersAsync();
        return HandleResponse(employers, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an employer by their ID.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <returns>The employer.</returns>
    /// <response code="200">Employer retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Employer not found.</response>
    [HttpGet("{employerId}")]
    [ProducesResponseType(typeof(AdminEmployerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployer(Guid employerId)
    {
        var (employer, statusCode, errorMessage) = await _adminEmployerService.GetEmployerAsync(employerId);
        return HandleResponse(employer, statusCode, errorMessage);
    }

    /// <summary>
    /// Updates an existing employer.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Employer updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Employer not found.</response>
    /// <response code="409">An employer with the specified email already exists.</response>
    [HttpPut("{employerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEmployer(Guid employerId, [FromBody] AdminUpdateEmployerDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminEmployerService.UpdateEmployerAsync(employerId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an employer.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (default is false).</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Employer deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Employer not found.</response>
    /// <response code="410">Employer is already deleted.</response>
    [HttpDelete("{employerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteEmployer(Guid employerId, [FromQuery] bool hardDelete = false)
    {
        var (success, statusCode, errorMessage) = await _adminEmployerService.DeleteEmployerAsync(employerId, hardDelete);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
