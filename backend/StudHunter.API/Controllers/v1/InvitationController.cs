using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1")]
public class InvitationController(IInvitationService invitationService) : BaseController
{
    [HttpGet("student/invitations")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetStudentInvitations([FromQuery] InvitationSearchFilter filter) =>
        HandleResult(await invitationService.GetInvitationsForStudentAsync(AuthorizedUserId, filter));

    [HttpPost("student/invitations")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SendResponse([FromBody] CreateInvitationDto dto) =>
        HandleResult(await invitationService.CreateInvitationAsync(AuthorizedUserId, dto, Invitation.InvitationType.Response));

    [HttpGet("employer/invitations")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> GetEmployerInvitations([FromQuery] InvitationSearchFilter filter) =>
        HandleResult(await invitationService.GetInvitationsForEmployerAsync(AuthorizedUserId, filter));

    [HttpPost("employer/invitations")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> SendOffer([FromBody] CreateInvitationDto dto) =>
        HandleResult(await invitationService.CreateInvitationAsync(AuthorizedUserId, dto, Invitation.InvitationType.Offer));

    [HttpPatch("invitations/{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id) =>
        HandleResult(await invitationService.ChangeStatusAsync(AuthorizedUserId, id, Invitation.InvitationStatus.Accepted));

    [HttpPatch("invitations/{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id) =>
        HandleResult(await invitationService.ChangeStatusAsync(AuthorizedUserId, id, Invitation.InvitationStatus.Rejected));

    [HttpPatch("invitations/{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id) =>
        HandleResult(await invitationService.ChangeStatusAsync(AuthorizedUserId, id, Invitation.InvitationStatus.Cancelled));
}