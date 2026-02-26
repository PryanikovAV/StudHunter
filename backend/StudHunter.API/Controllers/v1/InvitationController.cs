using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1/invitations")]
public class InvitationController(IInvitationService invitationService) : BaseController
{
    [Authorize(Roles = UserRoles.Student)]
    [HttpGet("student")]
    public async Task<IActionResult> GetStudentInvitations([FromQuery] InvitationSearchFilter filter) =>
        HandleResult(await invitationService.GetInvitationsForStudentAsync(AuthorizedUserId, filter));

    [Authorize(Roles = UserRoles.Student)]
    [HttpPost("student")]
    public async Task<IActionResult> SendResponse([FromBody] CreateResponseDto dto) =>
        HandleResult(await invitationService.CreateResponseAsync(AuthorizedUserId, dto));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpGet("employer")]
    public async Task<IActionResult> GetEmployerInvitations([FromQuery] InvitationSearchFilter filter) =>
        HandleResult(await invitationService.GetInvitationsForEmployerAsync(AuthorizedUserId, filter));

    [Authorize(Roles = UserRoles.Employer)]
    [HttpPost("employer")]
    public async Task<IActionResult> SendOffer([FromBody] CreateOfferDto dto) =>
        HandleResult(await invitationService.CreateOfferAsync(AuthorizedUserId, dto));

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