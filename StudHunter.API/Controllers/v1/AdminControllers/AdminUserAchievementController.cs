using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.UserAchievementDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing user achievements with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminUserAchievementController(AdminUserAchievementService adminUserAchievementService) : BaseController
{
    private readonly AdminUserAchievementService _adminUserAchievementService = adminUserAchievementService;

    /// <summary>
    /// Retrieves all user achievements.
    /// </summary>
    /// <returns>A list of all user achievements.</returns>
    /// <response code="200">User achievements retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserAchievementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUserAchievements()
    {
        var (achievements, statusCode, errorMessage) = await _adminUserAchievementService.GetAllUserAchievementsAsync();
        return HandleResponse(achievements, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a specific user achievement by user ID and achievement template order number.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="achievementTemplateId">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>The user achievement.</returns>
    /// <response code="200">User achievement retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">User achievement not found.</response>
    [HttpGet("{userId}/{achievementTemplateId}")]
    [ProducesResponseType(typeof(UserAchievementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAchievement(Guid userId, Guid achievementTemplateId)
    {
        var (achievement, statusCode, errorMessage) = await _adminUserAchievementService.GetUserAchievementAsync(userId, achievementTemplateId);
        return HandleResponse(achievement, statusCode, errorMessage);
    }

    /// <summary>
    /// Grants an achievement to a user.
    /// </summary>
    /// <param name="dto">The data transfer object containing user achievement details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">User achievement granted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">User or achievement template not found.</response>
    /// <response code="409">User achievement already exists.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GrantAchievement([FromBody] GrantAchievementDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminUserAchievementService.GrantAchievementAsync(dto.UserId, dto.AchievementTemplateId);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a user achievement by user ID and achievement template order number.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="achievementTemplateId">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">User achievement deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">User achievement not found.</response>
    [HttpDelete("{userId}/{achievementTemplateId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAchievement(Guid userId, Guid achievementTemplateId)
    {
        var (success, statusCode, errorMessage) = await _adminUserAchievementService.DeleteUserAchievementAsync(userId, achievementTemplateId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
