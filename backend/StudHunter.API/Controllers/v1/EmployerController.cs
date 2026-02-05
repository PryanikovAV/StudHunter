using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;
using StudHunter.API.Infrastructure;

namespace StudHunter.API.Controllers.v1;

[Authorize(Roles = UserRoles.Employer)]
public class EmployerController(IEmployerService employerService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetProfile() =>
        HandleResult(await employerService.GetEmployerAsync(AuthorizedUserId));

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateEmployerDto dto) =>
        HandleResult(await employerService.UpdateEmployerAsync(AuthorizedUserId, dto));

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount() =>
        HandleResult(await employerService.DeleteEmployerAsync(AuthorizedUserId));
}