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

    [HttpGet]
    public async Task<IActionResult> GetAllUserAchievements()
    {
        var (achievements, statusCode, errorMessage) = await _adminUserAchievementService.GetAllUserAchievementsAsync();
        return this.CreateAPIError(achievements, statusCode, errorMessage);
    }

    [HttpGet("{userId}/{achievementTemplateOrderNumber}")]
    public async Task<IActionResult> GetUserAchievement(Guid userId, int achievementTemplateOrderNumber)
    {
        var (achievement, statusCode, errorMessage) = await _adminUserAchievementService.GetUserAchievementAsync(userId, achievementTemplateOrderNumber);
        return this.CreateAPIError(achievement, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> GrantAchievement([FromBody] GrantAchievementDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, statusCode, errorMessage) = await _adminUserAchievementService.GrantAchievementAsync(dto.UserId, dto.AchievementTemplateOrderNumber);
        return CreateAPIError<UserAchievementDto>(success, statusCode, errorMessage);
    }

    [HttpDelete("{userId}/{achievementTemplateOrderNumber}")]
    public async Task<IActionResult> DeleteUserAchievement(Guid userId, int achievementTemplateOrderNumber)
    {
        var (success, statusCode, errorMessage) = await _adminUserAchievementService.DeleteUserAchievementAsync(userId, achievementTemplateOrderNumber);
        return CreateAPIError<UserAchievementDto>(success, statusCode, errorMessage);
    }
}
