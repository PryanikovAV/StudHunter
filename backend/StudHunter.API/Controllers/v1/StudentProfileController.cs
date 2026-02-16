using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize(Roles = UserRoles.Student)]
[Route("api/v1/student/profile")]
public class StudentProfileController(IStudentProfileService profileService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetFullProfile() =>
        HandleResult(await profileService.GetProfileAsync(AuthorizedUserId));

    [HttpPut]
    public async Task<IActionResult> UpdateFullProfile([FromBody] StudentProfileDto dto) =>
        HandleResult(await profileService.UpdateProfileAsync(AuthorizedUserId, dto));
}
