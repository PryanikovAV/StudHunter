using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.InvitationDto;
using StudHunter.API.Services;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing invitations.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class InvitationController(InvitationService invitationService) : BaseController
{
    private readonly InvitationService _invitationService = invitationService;

    /// <summary>
    /// Retrieves sent invitations for the authenticated user.
    /// </summary>
    /// <returns>A list of sent invitations.</returns>
    /// <response code="200">Invitations retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this invitations.</response>
    [HttpGet("sent")]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSentInvitationsByUser()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<InvitationDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (invitations, statusCode, errorMessage) = await _invitationService.GetInvitationsByUserAsync(authUserId, sent: true);
        return HandleResponse(invitations, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves received invitations for the authenticated user.
    /// </summary>
    /// <returns>A list of received invitations.</returns>
    /// <response code="200">Invitations retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("received")]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetReceivedInvitationsByUser()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<InvitationDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (invitations, statusCode, errorMessage) = await _invitationService.GetInvitationsByUserAsync(authUserId, sent: false);
        return HandleResponse(invitations, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an invitation by its ID for the authenticated user.
    /// </summary>
    /// <param name="invitationId">The unique identifier (GUID) of the invitation.</param>
    /// <returns>The invitation.</returns>
    /// <response code="200">Invitation retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Invitation not found.</response>
    [HttpGet("{invitationId}")]
    [ProducesResponseType(typeof(InvitationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvitation(Guid invitationId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<InvitationDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (invitation, statusCode, errorMessage) = await _invitationService.GetInvitationAsync(authUserId, invitationId);
        return HandleResponse(invitation, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    /// <param name="dto">The data transfer object containing invitation details.</param>
    /// <returns>The created invitation.</returns>
    /// <response code="201">Invitation created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to create this invitation.</response>
    /// <response code="404">Sender, receiver, vacancy or resume not found.</response>
    /// <response code="409">Invitation already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(InvitationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateInvitation([FromBody] CreateInvitationDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<InvitationDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<InvitationDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (invitation, statusCode, errorMessage) = await _invitationService.CreateInvitationAsync(authUserId, dto);
        return HandleResponse(invitation, statusCode, errorMessage, nameof(GetInvitation), new { invitationId = invitation?.Id });
    }

    /// <summary>
    /// Updates the status of an invitation.
    /// </summary>
    /// <param name="invitationId">The unique identifier (GUID) of the invitation.</param>
    /// <param name="dto">The data transfer object containing the updated status.</param>
    /// <param name="receiverId">The unique identifier (GUID) of the receiver.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Invitation status updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to update this invitation.</response>
    /// <response code="404">Invitation not found.</response>
    /// <response code="409">Invitation status cannot be changed.</response>
    [HttpPut("{invitationId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateInvitationStatus(Guid invitationId, [FromBody] UpdateInvitationDto dto, [FromQuery] Guid receiverId)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _invitationService.UpdateInvitationStatusAsync(receiverId, invitationId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
