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
public class UserAchievementService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Retrieves all achievements for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing a list of user achievements, an optional status code, and an optional error message.</returns>
    public async Task<(List<UserAchievementDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllUserAchievementsByUserAsync(Guid userId)
    {
        #region Serializers
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(User)));
        #endregion

        var userAchievements = await _context.UserAchievements
        .Where(ua => ua.UserId == userId)
        .Include(ua => ua.AchievementTemplate)
        .Select(ua => new UserAchievementDto
        {
            Id = ua.Id,
            UserId = ua.UserId,
            AchievementTemplateOrderNumber = ua.AchievementTemplate.OrderNumber,
            AchievementAt = ua.AchievementAt,
            AchievementName = ua.AchievementTemplate.Name,
            AchievementDescription = ua.AchievementTemplate.Description,
            IconUrl = ua.AchievementTemplate.IconUrl
        })
        .OrderBy(ua => ua.AchievementAt)
        .ToListAsync();

        return (userAchievements, null, null);
    }

    /// <summary>
    /// Grants an achievement to a user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="orderNumber">The order number of the achievement template.</param>
    /// <returns>A tuple indicating whether the operation was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> GrantAchievementAsync(Guid userId, int orderNumber)
    {
        #region Serializers
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(User)));

        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.OrderNumber == orderNumber);
        if (template == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(AchievementTemplate)));

        if (await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == template.Id))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(UserAchievement), "userId, achievementTemplateOrderNumber"));
        #endregion

        var userAchievement = new UserAchievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AchievementTemplateId = template.Id,
            AchievementAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<UserAchievement>();

        return (success, statusCode, errorMessage);
    }
}
