using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing invitations with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminInvitationController(AdminInvitationService adminInvitationService) : BaseController
{
    private readonly AdminInvitationService _adminInvitationService = adminInvitationService;

    /// <summary>
    /// Retrieves all invitations.
    /// </summary>
    /// <returns>A list of all invitations.</returns>
    /// <response code="200">Invitations retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllInvitations()
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetAllInvitationsAsync();
        return CreateAPIError(invitations, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves sent invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of sent invitations.</returns>
    /// <response code="200">Invitations retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet("sent/{userId}")]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSentInvitationsByUser(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: true);
        return CreateAPIError(invitations, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves received invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of received invitations.</returns>
    /// <response code="200">Invitations retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet("received/{userId}")]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReceivedInvitationsByUser(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: false);
        return CreateAPIError(invitations, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an invitation.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Invitation deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Invitation not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInvitation(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminInvitationService.DeleteInvitationAsync(id);
        return CreateAPIError<InvitationDto>(success, statusCode, errorMessage);
    }
}
