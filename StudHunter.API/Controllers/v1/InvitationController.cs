using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class InvitationController(InvitationService invitationService) : ControllerBase
{
    private readonly InvitationService _invitationService = invitationService;

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentInvitationsByUser(Guid userId)
    {
        var invitations = await _invitationService.GetInvitationsByUserAsync(userId, sent: true);
        return Ok(invitations);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedInvitations(Guid userId)
    {
        var invitations = await _invitationService.GetInvitationsByUserAsync(userId, sent: false);
        return Ok(invitations);
    }
    // TODO: add jwt
    // TODO: check/fix 'return nameof(GetSentInvitationsByUser)'
    [HttpPost]
    public async Task<IActionResult> CreateInvitation([FromBody] CreateInvitationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var senderId = Guid.NewGuid();  // <- Change this !!! (get from Jwt token)
        var (invitation, error) = await _invitationService.CreateInvitationAsync(senderId, dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetSentInvitationsByUser), new { userId = invitation!.SenderId }, invitation);
    }
    // TODO: add jwt
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateInvitationStatus(Guid id, [FromBody] UpdateInvitationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var receiverId = Guid.NewGuid();  // <- Change this !!! (get from Jwt token)
        var (success, error) = await _invitationService.UpdateInvitationStatusAsync(id, receiverId, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
