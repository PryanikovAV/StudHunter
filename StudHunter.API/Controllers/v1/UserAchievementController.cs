using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing user achievements.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class UserAchievementController(UserAchievementService userAchievementService) : BaseController
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

    /// <summary>
    /// Retrieves all achievements for the authenticated user.
    /// </summary>
    /// <returns>A list of user achievements.</returns>
    /// <response code="200">User achievements retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("user/achievements")]
    [ProducesResponseType(typeof(List<UserAchievementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllUserAchievements()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return CreateAPIError<List<UserAchievementDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (achievements, statusCode, errorMessage) = await _userAchievementService.GetAllUserAchievementsByUserAsync(userId);
        return CreateAPIError(achievements, statusCode, errorMessage);
    }
}
