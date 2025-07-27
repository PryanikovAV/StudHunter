using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class UserAchievementController(UserAchievementService userAchievementService) : ControllerBase
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

    [HttpGet("user/{userId}/achievements")]
    public async Task<IActionResult> GetUserAchievements(Guid id)
    {
        var achievements = await _userAchievementService.GetAllUserAchievementsAsync(id);
        return Ok(achievements);
    }
}
