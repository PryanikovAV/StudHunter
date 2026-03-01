using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/notifications")]
public class AdminNotificationController(IAdminNotificationService adminNotificationService) : BaseController
{
    [HttpPost("mass-mail")]
    public async Task<IActionResult> SendMassNotification([FromBody] SendMassNotificationRequest request) =>
        HandleResult(await adminNotificationService.SendMassNotificationAsync(request));
}