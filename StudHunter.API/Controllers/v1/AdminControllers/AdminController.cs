using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.AdminDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminController(AdminService administratorService) : BaseController
{
    private readonly AdminService _administratorService = administratorService;

    /// <summary>
    /// Retrieves all administrators.
    /// </summary>
    /// <returns>A list of administrators.</returns>
    /// <response code="200">Administrators retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AdminDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllAdministrators()
    {
        var (administrators, statusCode, errorMessage) = await _administratorService.GetAllAdministratorsAsync();
        return HandleResponse(administrators, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an administrator by their ID.
    /// </summary>
    /// <param name="adminId">The unique identifier (GUID) of the administrator.</param>
    /// <returns>The administrator.</returns>
    /// <response code="200">Administrator retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Admin not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAdministrator(Guid adminId)
    {
        var (administrator, statusCode, errorMessage) = await _administratorService.GetAdministratorAsync(adminId);
        return HandleResponse(administrator, statusCode, errorMessage);
    }

    /// <summary>
    /// Updates an existing administrator.
    /// </summary>
    /// <param name="adminId">The unique identifier (GUID) of the administrator.</param>
    /// <param name="dto">The data transfer object containing updated administrator details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Administrator updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Admin not found.</response>
    /// <response code="409">An admin with the specified email already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAdministrator(Guid adminId, [FromBody] UpdateAdminDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _administratorService.UpdateAdministratorAsync(adminId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
