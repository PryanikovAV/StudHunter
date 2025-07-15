using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class UserAchievementController(UserAchievementService userAchievementService) : ControllerBase
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

    [HttpGet("user-achievements")]
    public async Task<IActionResult> GetUserAchievements(Guid id)
    {
        var achievements = await _userAchievementService.GetUserAchievementsAsync(id);
        return Ok(achievements);
    }
}
