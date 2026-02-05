using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1;

[Authorize(Roles = UserRoles.Employer)]
[Route("api/v1/employer/vacancies")]
public class VacancyController(IVacancyService vacancyService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyVacancies([FromQuery] PaginationParams paging, [FromQuery] bool includeDeleted = true) =>
        HandleResult(await vacancyService.GetAllVacanciesAsync(AuthorizedUserId, paging, includeDeleted));

    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetById(Guid vacancyId) =>
        HandleResult(await vacancyService.GetVacancyById(vacancyId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpdateVacancyDto dto) =>
        HandleResult(await vacancyService.CreateVacancyAsync(AuthorizedUserId, dto));

    [HttpPut("{vacancyId:guid}")]
    public async Task<IActionResult> Update(Guid vacancyId, [FromBody] UpdateVacancyDto dto) =>
        HandleResult(await vacancyService.UpdateVacancyAsync(vacancyId, dto, AuthorizedUserId));

    [HttpDelete("{vacancyId:guid}")]
    public async Task<IActionResult> Delete(Guid vacancyId) =>
       HandleResult(await vacancyService.SoftDeleteVacancyAsync(vacancyId, AuthorizedUserId));

    [HttpPost("{vacancyId:guid}/restore")]
    public async Task<IActionResult> Restore(Guid vacancyId) =>
        HandleResult(await vacancyService.RestoreVacancyAsync(vacancyId, AuthorizedUserId));
}

[Authorize(Roles = UserRoles.Student)]
[Route("api/v1/vacancies")]
public class StudentVacancyController(IVacancyService vacancyService, IInvitationService invitationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] VacancySearchFilter filter) =>
        HandleResult(await vacancyService.SearchVacancyAsync(filter, AuthorizedUserId));

    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetDetails(Guid vacancyId) =>
        HandleResult(await vacancyService.GetVacancyById(vacancyId, AuthorizedUserId));

    [HttpPost("{vacancyId:guid}/apply")]
    public async Task<IActionResult> Apply(Guid vacancyId, [FromBody] string? message)
    {
        var vacancyResult = await vacancyService.GetVacancyById(vacancyId, AuthorizedUserId);
        if (!vacancyResult.IsSuccess)
            return HandleResult(vacancyResult);

        if (vacancyResult.Value!.IsCommunicationBlocked)
            return Forbid(ErrorMessages.CommunicationBlocked());

        var dto = new CreateInvitationDto(
            ReceiverId: vacancyResult.Value!.EmployerId,
            VacancyId: vacancyId,
            ResumeId: null,
            Message: message
        );

        return HandleResult(await invitationService.CreateInvitationAsync(AuthorizedUserId, dto, Invitation.InvitationType.Response));
    }
}