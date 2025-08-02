using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Admin;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
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
        return CreateAPIError(administrators, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an administrator by their ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the administrator.</param>
    /// <returns>The administrator.</returns>
    /// <response code="200">Administrator retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Admin not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAdministrator(Guid id)
    {
        var (administrator, statusCode, errorMessage) = await _administratorService.GetAdministratorAsync(id);
        return CreateAPIError(administrator, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new administrator.
    /// </summary>
    /// <param name="dto">The data transfer object containing administrator details.</param>
    /// <returns>The created administrator.</returns>
    /// <response code="201">Administrator created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="409">An admin with the specified email already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AdminDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAdministrator([FromBody] CreateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (administrator, statusCode, errorMessage) = await _administratorService.CreateAdministratorAsync(dto);
        return CreateAPIError(administrator, statusCode, errorMessage, nameof(GetAdministrator), new { id = administrator?.Id });
    }

    /// <summary>
    /// Updates an existing administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the administrator.</param>
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
    public async Task<IActionResult> UpdateAdministrator(Guid id, [FromBody] UpdateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request data." });

        var (success, statusCode, errorMessage) = await _administratorService.UpdateAdministratorAsync(id, dto);
        return CreateAPIError<AdminDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the administrator.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (default is false).</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Administrator deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Admin not found.</response>
    /// <response code="410">Admin is already deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteAdministrator(Guid id, [FromQuery] bool hardDelete = false)
    {
        var (success, statusCode, errorMessage) = await _administratorService.DeleteAdministratorAsync(id, hardDelete);
        return CreateAPIError<AdminDto>(success, statusCode, errorMessage);
    }
}
