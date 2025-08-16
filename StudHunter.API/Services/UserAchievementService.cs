using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing user achievements.
/// </summary>
public class UserAchievementService(StudHunterDbContext context) : BaseUserAchievementService(context)
{
    /// <summary>
    /// Retrieves all achievements for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing a list of user achievements, an optional status code, and an optional error message.</returns>
    public async Task<(List<UserAchievementDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllUserAchievementsByUserAsync(Guid userId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(User)));

        var userAchievements = await _context.UserAchievements
        .Where(ua => ua.UserId == userId)
        .Include(ua => ua.AchievementTemplate)
        .Select(ua => MapToUserAchievementDto(ua))
        .OrderBy(ua => ua.AchievementAt)
        .ToListAsync();

        return (userAchievements, null, null);
    }
}
