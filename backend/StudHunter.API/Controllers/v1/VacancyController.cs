using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize(Roles = UserRoles.Employer)]
[Route("api/v1/employer/vacancies")]
public class EmployerVacancyController(IVacancyService vacancyService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyVacancies([FromQuery] PaginationParams paging, [FromQuery] bool includeDeleted = true) =>
        HandleResult(await vacancyService.GetMyVacanciesAsync(AuthorizedUserId, paging, includeDeleted));

    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetVacancy(Guid vacancyId) =>
        HandleResult(await vacancyService.GetVacancyByIdForEditAsync(vacancyId, AuthorizedUserId));

    [HttpPost]
    public async Task<IActionResult> CreateVacancy([FromBody] VacancyFillDto dto) =>
        HandleResult(await vacancyService.CreateVacancyAsync(AuthorizedUserId, dto));

    [HttpPut("{vacancyId:guid}")]
    public async Task<IActionResult> UpdateVacancy(Guid vacancyId, [FromBody] VacancyFillDto dto) =>
        HandleResult(await vacancyService.UpdateVacancyAsync(vacancyId, AuthorizedUserId, dto));

    [HttpDelete("{vacancyId:guid}")]
    public async Task<IActionResult> DeleteVacancy(Guid vacancyId) =>
        HandleResult(await vacancyService.SoftDeleteVacancyAsync(vacancyId, AuthorizedUserId));

    [HttpPost("{vacancyId:guid}/restore")]
    public async Task<IActionResult> RestoreVacancy(Guid vacancyId) =>
        HandleResult(await vacancyService.RestoreVacancyAsync(vacancyId, AuthorizedUserId));
}

[Authorize(Roles = UserRoles.Student)]
[Route("api/v1/vacancies")]
public class VacancyController(IVacancyService vacancyService, IInvitationService invitationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> SearchVacancies([FromQuery] VacancySearchFilter filter) =>
        HandleResult(await vacancyService.SearchVacanciesAsync(filter, AuthorizedUserId));

    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetVacancyDetails(Guid vacancyId) =>
        HandleResult(await vacancyService.GetVacancyDetailsAsync(vacancyId, AuthorizedUserId));

    [HttpPost("{vacancyId:guid}/apply")]
    public async Task<IActionResult> ApplyToVacancy(Guid vacancyId, [FromBody] ApplyToVacancyRequest request)
    {
        var vacancyResult = await vacancyService.GetVacancyDetailsAsync(vacancyId, AuthorizedUserId);
        if (!vacancyResult.IsSuccess)
            return HandleResult(vacancyResult);

        var dto = new CreateResponseDto(
            VacancyId: vacancyId,
            ResumeId: request.ResumeId,
            Message: request.Message
        );

        return HandleResult(await invitationService.CreateResponseAsync(AuthorizedUserId, dto));
    }
}