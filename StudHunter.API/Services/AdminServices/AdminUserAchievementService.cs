using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing user achievements with administrative privileges.
/// </summary>
public class AdminUserAchievementService(StudHunterDbContext context) : UserAchievementService(context)
{
    /// <summary>
    /// Retrieves all user achievements.
    /// </summary>
    /// <returns>A tuple containing a list of all user achievements, an optional status code, and an optional error message.</returns>
    public async Task<(List<UserAchievementDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllUserAchievementsAsync()
    {
        var userAchievements = await _context.UserAchievements
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
    /// Retrieves a specific user achievement by user ID and achievement template order number.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="orderNumber">The order number of the achievement template.</param>
    /// <returns>A tuple containing the user achievement, an optional status code, and an optional error message.</returns>
    public async Task<(UserAchievementDto? Entity, int? StatusCode, string? ErrorMessage)> GetUserAchievementAsync(Guid userId, int orderNumber)
    {
        var userAchievement = await _context.UserAchievements
        .Include(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplate.OrderNumber == orderNumber);

        #region Serializers
        if (userAchievement == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(UserAchievement)));
        #endregion

        return (new UserAchievementDto
        {
            Id = userAchievement.Id,
            UserId = userAchievement.UserId,
            AchievementTemplateOrderNumber = userAchievement.AchievementTemplate.OrderNumber,
            AchievementAt = userAchievement.AchievementAt,
            AchievementName = userAchievement.AchievementTemplate.Name,
            AchievementDescription = userAchievement.AchievementTemplate.Description,
            IconUrl = userAchievement.AchievementTemplate.IconUrl
        }, null, null);
    }

    /// <summary>
    /// Deletes a user achievement by user ID and achievement template order number.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="orderNumber">The order number of the achievement template.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteUserAchievementAsync(Guid userId, int orderNumber)
    {
        var userAchievement = await _context.UserAchievements
        .Include(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplate.OrderNumber == orderNumber);

        #region Serializers
        if (userAchievement == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(UserAchievement)));
        #endregion

        return await DeleteEntityAsync<UserAchievement>(userAchievement.Id, hardDelete: true);
    }
}
