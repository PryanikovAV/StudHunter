using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/invitations")]
public class AdminInvitationController(IAdminInvitationService adminInvitationService) : BaseController
{
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserInvitations(Guid userId, [FromQuery] InvitationSearchFilter filter) =>
        HandleResult(await adminInvitationService.GetInvitationsAsync(userId, filter));

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ForceUpdateStatus(Guid id, [FromQuery] Invitation.InvitationStatus status) =>
        HandleResult(await adminInvitationService.UpdateStatusForcedAsync(id, status));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> HardDelete(Guid id) =>
        HandleResult(await adminInvitationService.HardDeleteAsync(id));
}