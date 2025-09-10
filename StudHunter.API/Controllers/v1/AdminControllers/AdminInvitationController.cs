using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.InvitationDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing invitations with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllInvitations()
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetAllInvitationsAsync();
        return HandleResponse(invitations, statusCode, errorMessage);
    }

    [HttpGet("{invitationId}")]
    [ProducesResponseType(typeof(InvitationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvitation(Guid invitationId)
    {
        var (invitation, statusCode, errorMessage) = await _adminInvitationService.GetInvitationAsync(invitationId);
        return HandleResponse(invitation, statusCode, errorMessage);
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSentInvitationsByUser(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: true);
        return HandleResponse(invitations, statusCode, errorMessage);
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReceivedInvitationsByUser(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: false);
        return HandleResponse(invitations, statusCode, errorMessage);
    }

    [HttpPut("{invitationId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateInvitationStatus(Guid invitationId, [FromBody] UpdateInvitationDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminInvitationService.UpdateInvitationStatusAsync(invitationId, dto.Status);
        return HandleResponse(success, statusCode, errorMessage, nameof(Invitation));
    }

    /// <summary>
    /// Deletes an invitation.
    /// </summary>
    /// <param name="invitationId">The unique identifier (GUID) of the invitation.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Invitation deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Invitation not found.</response>
    [HttpDelete("{invitationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInvitation(Guid invitationId)
    {
        var (success, statusCode, errorMessage) = await _adminInvitationService.DeleteInvitationAsync(invitationId);
        return HandleResponse(success, statusCode, errorMessage, nameof(Invitation));
    }
}
