using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class AdministratorInvitationController(AdministratorInvitationService administratorInvitationService) : ControllerBase
{
    private readonly AdministratorInvitationService _administratorInvitationService = administratorInvitationService;

    [HttpGet("invitations")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAllInvitations()
    {
        var invitations = await _administratorInvitationService.GetAllInvitationsAsync();
        return Ok(invitations);
    }

    [HttpGet("user/{userId}/sent")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetSentInvitations(Guid userId)
    {
        var invitations = await _administratorInvitationService.GetInvitationsByUserAsync(userId, sent: true);
        return Ok(invitations);
    }

    [HttpGet("user/{userId}/received")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetReceivedInvitations(Guid userId)
    {
        var invitations = await _administratorInvitationService.GetInvitationsByUserAsync(userId, sent: false);
        return Ok(invitations);
    }

    [HttpDelete("ivitation/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteInvitation(Guid id)
    {
        var (success, error) = await _administratorInvitationService.DeleteInvitationAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
