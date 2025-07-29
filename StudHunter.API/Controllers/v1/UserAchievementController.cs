using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class UserAchievementController(UserAchievementService userAchievementService) : BaseController
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

    [HttpGet("user/{userId}/achievements")]
    public async Task<IActionResult> GetAllAchievements()
    {
        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (achievements, statusCode, errorMessage) = await _userAchievementService.GetAllUserAchievementsAsync(userId);
        return this.CreateAPIError(achievements, statusCode, errorMessage);
    }
}
