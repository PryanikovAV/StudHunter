using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;
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
    /// Retrieves sent invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of sent invitations.</returns>
    /// <response code="200">Invitations retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("sent/{userId}")]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSentInvitationsByUser(Guid userId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<InvitationDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (userId != authUserId)
            return HandleResponse<InvitationDto>(null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get sent invitations", nameof(Invitation)));

        var (invitations, statusCode, errorMessage) = await _invitationService.GetInvitationsByUserAsync(userId, sent: true);
        return HandleResponse(invitations, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves received invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of received invitations.</returns>
    /// <response code="200">Invitations retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("received/{userId}")]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReceivedInvitationsByUser(Guid userId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<InvitationDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (userId != authUserId)
            return HandleResponse<InvitationDto>(null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get received invitations", nameof(Invitation)));

        var (invitations, statusCode, errorMessage) = await _invitationService.GetInvitationsByUserAsync(userId, sent: false);
        return HandleResponse(invitations, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an invitation by its ID for the authenticated user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <returns>The invitation.</returns>
    /// <response code="200">Invitation retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Invitation not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvitationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvitation(Guid id)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<InvitationDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (invitation, statusCode, errorMessage) = await _invitationService.GetInvitationAsync(authUserId, id);
        return HandleResponse(invitation, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    /// <param name="dto">The data transfer object containing invitation details.</param>
    /// <returns>The created invitation.</returns>
    /// <response code="201">Invitation created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Sender, receiver, vacancy, or resume not found.</response>
    /// <response code="409">An invitation with the specified senderId, receiverId, and vacancyId/resumeId already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(InvitationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
        return HandleResponse(invitation, statusCode, errorMessage, nameof(GetInvitation), new { id = invitation?.Id });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <param name="receiverId"></param>
    /// <returns></returns>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Invitation status updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Invitation not found.</response>
    /// <response code="409">Invitation status cannot be changed.</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateInvitationStatus([FromQuery] Guid receiverId, Guid id, [FromBody] UpdateInvitationDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<ResumeDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _invitationService.UpdateInvitationStatusAsync(receiverId, id, dto);
        return HandleResponse(success, statusCode, errorMessage, nameof(Invitation));
    }
}
