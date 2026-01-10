using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1/invitations")]
public class InvitationController(IInvitationService invitationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyInvitations([FromQuery] InvitationSearchFilter filter) =>
        HandleResult(await invitationService.GetInvitationsAsync(AuthorizedUserId, filter));

    [HttpPatch("{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id) =>
        HandleResult(await invitationService.ChangeStatusAsync(AuthorizedUserId, id, Invitation.InvitationStatus.Accepted));

    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id) =>
        HandleResult(await invitationService.ChangeStatusAsync(AuthorizedUserId, id, Invitation.InvitationStatus.Rejected));

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id) =>
        HandleResult(await invitationService.ChangeStatusAsync(AuthorizedUserId, id, Invitation.InvitationStatus.Cancelled));
}