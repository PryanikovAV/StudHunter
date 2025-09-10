using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.UserAchievementDto;
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
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (achievements, statusCode, errorMessage) = await _userAchievementService.GetAllUserAchievementsByUserAsync(authUserId);
        return HandleResponse(achievements, statusCode, errorMessage);
    }
}
