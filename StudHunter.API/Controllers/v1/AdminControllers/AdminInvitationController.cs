using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminInvitationController(AdminInvitationService adminInvitationService) : ControllerBase
{
    private readonly AdminInvitationService _adminInvitationService = adminInvitationService;

    [HttpGet("invitations")]
    public async Task<IActionResult> GetAllInvitations()
    {
        var invitations = await _adminInvitationService.GetAllInvitationsAsync();
        return Ok(invitations);
    }

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentInvitations(Guid userId)
    {
        var invitations = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: true);
        return Ok(invitations);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedInvitations(Guid userId)
    {
        var invitations = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: false);
        return Ok(invitations);
    }

    [HttpDelete("ivitation/{id}")]
    public async Task<IActionResult> DeleteInvitation(Guid id)
    {
        var (success, error) = await _adminInvitationService.DeleteInvitationAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
