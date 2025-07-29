using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class InvitationController(InvitationService invitationService) : BaseController
{
    private readonly InvitationService _invitationService = invitationService;

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentInvitationsByUser(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _invitationService.GetInvitationsByUserAsync(userId, sent: true);
        return this.CreateAPIError(invitations, statusCode, errorMessage);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedInvitations(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _invitationService.GetInvitationsByUserAsync(userId, sent: false);
        return this.CreateAPIError(invitations, statusCode, errorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> GetInvitation(Guid id)
    {
        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT 
        var (invitation, statusCode, errorMessage) = await _invitationService.GetInvitationAsync(id, userId);
        return CreateAPIError(invitation, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvitation([FromBody] CreateInvitationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var senderId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (invitation, statusCode, errorMessage) = await _invitationService.CreateInvitationAsync(senderId, dto);
        return this.CreateAPIError(invitation, statusCode, errorMessage, nameof(GetInvitation), new { id = invitation?.Id });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateInvitationStatus(Guid id, [FromBody] UpdateInvitationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var receiverId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (invitation, statusCode, errorMessage) = await _invitationService.UpdateInvitationStatusAsync(id, receiverId, dto);
        return this.CreateAPIError<InvitationDto>(invitation, statusCode, errorMessage);
    }
}
