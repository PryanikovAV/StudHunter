using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services.AdministratorServices;


namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class AdministratorUserAchievementController(AdministratorUserAchievementService administratorUserAchievementService) : ControllerBase
{
    private readonly AdministratorUserAchievementService _administratorUserAchievementService = administratorUserAchievementService;

    [HttpGet("user-achievements")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAllUserAchievements()
    {
        var achievements = await _administratorUserAchievementService.GetAllUserAchievementsAsync();
        return Ok(achievements);
    }

    [HttpDelete("user-achievement/{userId}/{achievementTemplateId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteUserAchievement(Guid userId, int achievementTemplateId)
    {
        var (success, error) = await _administratorUserAchievementService.DeleteUserAchievementAsync(userId, achievementTemplateId);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
