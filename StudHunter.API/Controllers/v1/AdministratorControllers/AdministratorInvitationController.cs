using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdministratorInvitationController(AdministratorInvitationService administratorInvitationService) : ControllerBase
{
    private readonly AdministratorInvitationService _administratorInvitationService = administratorInvitationService;

    [HttpGet("invitations")]
    public async Task<IActionResult> GetAllInvitations()
    {
        var invitations = await _administratorInvitationService.GetAllInvitationsAsync();
        return Ok(invitations);
    }

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentInvitations(Guid userId)
    {
        var invitations = await _administratorInvitationService.GetInvitationsByUserAsync(userId, sent: true);
        return Ok(invitations);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedInvitations(Guid userId)
    {
        var invitations = await _administratorInvitationService.GetInvitationsByUserAsync(userId, sent: false);
        return Ok(invitations);
    }

    [HttpDelete("ivitation/{id}")]
    public async Task<IActionResult> DeleteInvitation(Guid id)
    {
        var (success, error) = await _administratorInvitationService.DeleteInvitationAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
