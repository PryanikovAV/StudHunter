using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminInvitationController(AdminInvitationService adminInvitationService) : BaseController
{
    private readonly AdminInvitationService _adminInvitationService = adminInvitationService;

    [HttpGet("invitations")]
    public async Task<IActionResult> GetAllInvitations()
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetAllInvitationsAsync();
        return this.CreateAPIError(invitations, statusCode, errorMessage);
    }

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentInvitationsByUser(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: true);
        return this.CreateAPIError(invitations, statusCode, errorMessage);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedInvitationsByUser(Guid userId)
    {
        var (invitations, statusCode, errorMessage) = await _adminInvitationService.GetInvitationsByUserAsync(userId, sent: false);
        return this.CreateAPIError(invitations, statusCode, errorMessage);
    }

    [HttpDelete("ivitation/{id}")]
    public async Task<IActionResult> DeleteInvitation(Guid id)
    {
        var (invitation, statusCode, messageError) = await _adminInvitationService.DeleteInvitationAsync(id);
        return this.CreateAPIError<InvitationDto>(invitation, statusCode, messageError);
    }
}
