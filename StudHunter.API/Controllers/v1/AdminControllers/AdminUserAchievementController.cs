using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminUserAchievementController(AdminUserAchievementService adminUserAchievementService) : BaseController
{
    private readonly AdminUserAchievementService _adminUserAchievementService = adminUserAchievementService;

    [HttpGet("user-achievements")]
    public async Task<IActionResult> GetAllUserAchievements()
    {
        var (achievements, statusCode, errorMessage) = await _adminUserAchievementService.GetAllUserAchievementsAsync();
        return this.CreateAPIError(achievements, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> GrantAchievement([FromBody] GrantAchievementDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, statusCode, errorMessage) = await _adminUserAchievementService.GrantAchievementAsync(dto.UserId, dto.AchievementTemplateOrderNumber);
        if (!success)
        return CreateAPIError(success, statusCode, errorMessage);
    }

    [HttpDelete("user-achievement/{userId}/{achievementTemplateId}")]
    public async Task<IActionResult> DeleteUserAchievement(Guid userId, Guid achievementTemplateId)
    {
        var (success, error) = await _adminUserAchievementService.DeleteUserAchievementAsync(userId, achievementTemplateId);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
