using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1;

[Authorize(Roles = UserRoles.Student)]
[Route("api/v1/my-resume")]
public class ResumeController(IResumeService resumeService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyResume() =>
        HandleResult(await resumeService.GetResumeByStudentIdAsync(AuthorizedUserId, null, true));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpdateResumeDto dto) =>
        HandleResult(await resumeService.CreateResumeAsync(AuthorizedUserId, dto));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateResumeDto dto) =>
        HandleResult(await resumeService.UpdateResumeAsync(AuthorizedUserId, dto));

    [HttpDelete]
    public async Task<IActionResult> Delete() =>
        HandleResult(await resumeService.SoftDeleteResumeAsync(AuthorizedUserId));

    [HttpPost("restore")]
    public async Task<IActionResult> Restore() =>
        HandleResult(await resumeService.RestoreResumeAsync(AuthorizedUserId));
}

[Authorize(Roles = UserRoles.Employer)]
[Route("api/v1/resumes")]
public class EmployerResumeController(IResumeService resumeService, IInvitationService invitationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ResumeSearchFilter filter) =>
        HandleResult(await resumeService.SearchResumesAsync(filter, AuthorizedUserId));

    [HttpGet("{studentId:guid}")]
    public async Task<IActionResult> GetStudentResume(Guid studentId) =>
        HandleResult(await resumeService.GetResumeByStudentIdAsync(studentId, AuthorizedUserId));

    [HttpPost("{studentId:guid}/invite")]
    public async Task<IActionResult> Invite(Guid studentId, [FromBody] CreateOfferRequest request)
    {
        var resumeResult = await resumeService.GetResumeByStudentIdAsync(studentId, AuthorizedUserId);
        if (!resumeResult.IsSuccess)
            return HandleResult(resumeResult);

        var dto = new CreateInvitationDto(
            ReceiverId: studentId,
            VacancyId: request.VacancyId,
            ResumeId: resumeResult.Value!.Id,
            Message: request.Message
        );

        return HandleResult(await invitationService.CreateInvitationAsync(AuthorizedUserId, dto, Invitation.InvitationType.Offer));
    }
}

public record CreateOfferRequest(Guid? VacancyId, string? Message);