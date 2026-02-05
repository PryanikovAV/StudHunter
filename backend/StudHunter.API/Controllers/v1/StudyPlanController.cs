using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers;

[Authorize(Roles = UserRoles.Student)]
public class StudyPlanController(IStudyPlanService studyPlanService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyStudyPlan() =>
        HandleResult(await studyPlanService.GetByStudentIdAsync(AuthorizedUserId));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStudyPlanDto dto) =>
        HandleResult(await studyPlanService.UpdateAsync(AuthorizedUserId, dto));
}