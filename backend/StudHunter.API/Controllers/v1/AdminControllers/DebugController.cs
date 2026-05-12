using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[ApiController]
[Route("api/v1/debug")]
public class DebugController(IAdminEmployerService adminService, IWebHostEnvironment env) : ControllerBase
{
    [HttpPatch("employers/{id:guid}/stage")]
    public async Task<IActionResult> SetStage(Guid id, [FromQuery] User.AccountStatus stage)
    {
        // TODO: убрать на релизе
        //if (!env.IsDevelopment())
        //    return NotFound();

        var result = await adminService.SetRegistrationStageAsync(id, stage);
        return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
    }
}
