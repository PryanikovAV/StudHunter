using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminUserAchievementController(AdminUserAchievementService adminUserAchievementService) : ControllerBase
{
    private readonly AdminUserAchievementService _adminUserAchievementService = adminUserAchievementService;

    [HttpGet("user-achievements")]
    public async Task<IActionResult> GetAllUserAchievements()
    {
        var achievements = await _adminUserAchievementService.GetAllUserAchievementsAsync();
        return Ok(achievements);
    }

    [HttpPost("grant")]
    public async Task<IActionResult> GrantAchievement([FromBody] GrantAchievementDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminUserAchievementService.GrantAchievementAsync(dto.UserId, dto.AchievementTemplateId);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return Ok();
    }

    [HttpDelete("user-achievement/{userId}/{achievementTemplateId}")]
    public async Task<IActionResult> DeleteUserAchievement(Guid userId, int achievementTemplateId)
    {
        var (success, error) = await _adminUserAchievementService.DeleteUserAchievementAsync(userId, achievementTemplateId);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
