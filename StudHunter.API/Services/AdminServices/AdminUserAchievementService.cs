using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.UserAchievementDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing user achievements with administrative privileges.
/// </summary>
public class AdminUserAchievementService(StudHunterDbContext context) : BaseUserAchievementService(context)
{
    /// <summary>
    /// Retrieves all user achievements.
    /// </summary>
    /// <returns>A tuple containing a list of all user achievements, an optional status code, and an optional error message.</returns>
    public async Task<(List<UserAchievementDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllUserAchievementsAsync()
    {
        var userAchievements = await _context.UserAchievements
            .Include(ua => ua.AchievementTemplate)
            .Select(ua => MapToUserAchievementDto(ua))
            .OrderBy(ua => ua.AchievementAt)
            .ToListAsync();

        return (userAchievements, null, null);
    }

    /// <summary>
    /// Retrieves a specific user achievement by user ID and achievement template order number.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="achievementTemplateId">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>A tuple containing the user achievement, an optional status code, and an optional error message.</returns>
    public async Task<(UserAchievementDto? Entity, int? StatusCode, string? ErrorMessage)> GetUserAchievementAsync(Guid userId, Guid achievementTemplateId)
    {
        var userAchievement = await _context.UserAchievements
            .Include(ua => ua.AchievementTemplate)
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == achievementTemplateId);

        if (userAchievement == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(UserAchievement)));

        return (MapToUserAchievementDto(userAchievement), null, null);
    }

    /// <summary>
    /// Deletes a user achievement by user ID and achievement template order number.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="achievementTemplateId">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteUserAchievementAsync(Guid userId, Guid achievementTemplateId)
    {
        var userAchievement = await _context.UserAchievements
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == achievementTemplateId);
        if (userAchievement == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(UserAchievement)));

        _context.UserAchievements.Remove(userAchievement);
        return await SaveChangesAsync<UserAchievementDto>();
    }
}
