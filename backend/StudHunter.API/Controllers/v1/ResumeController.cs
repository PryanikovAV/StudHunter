using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize(Roles = UserRoles.Student)]
[Route("api/v1/my-resume")]
public class ResumeController(IResumeService resumeService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyResume() =>
        HandleResult(await resumeService.GetMyResumeAsync(AuthorizedUserId));

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] ResumeFillDto dto) =>
        HandleResult(await resumeService.UpsertResumeAsync(AuthorizedUserId, dto));

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
        HandleResult(await resumeService.GetResumeForEmployerAsync(studentId, AuthorizedUserId));

    [HttpPost("{studentId:guid}/invite")]
    public async Task<IActionResult> Invite(Guid studentId, [FromBody] CreateOfferRequest request)
    {
        var resumeResult = await resumeService.GetResumeForEmployerAsync(studentId, AuthorizedUserId);
        if (!resumeResult.IsSuccess)
            return HandleResult(resumeResult);

        var dto = new CreateOfferDto(
            StudentId: studentId,
            VacancyId: request.VacancyId,
            Message: request.Message
        );

        return HandleResult(await invitationService.CreateOfferAsync(AuthorizedUserId, dto));
    }
}