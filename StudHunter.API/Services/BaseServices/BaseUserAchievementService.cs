using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseUserAchievementService(StudHunterDbContext context) : BaseService(context)
{
    public static UserAchievementDto MapToUserAchievementDto(UserAchievement userAchievement)
    {
        return new UserAchievementDto
        {
            Id = userAchievement.Id,
            UserId = userAchievement.UserId,
            AchievementTemplateId = userAchievement.AchievementTemplateId,
            AchievementAt = userAchievement.AchievementAt,
            AchievementName = userAchievement.AchievementTemplate.Name,
            AchievementDescription = userAchievement.AchievementTemplate.Description,
            IconUrl = userAchievement.AchievementTemplate.IconUrl
        };
    }

    /// <summary>
    /// Grants an achievement to a user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="achievementTemplateId">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>A tuple indicating whether the operation was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> GrantAchievementAsync(Guid userId, Guid achievementTemplateId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(User)));

        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.Id == achievementTemplateId);
        if (template == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(AchievementTemplate)));

        if (await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == achievementTemplateId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(UserAchievement), "userId, achievementTemplateId"));

        var userAchievement = new UserAchievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AchievementTemplateId = achievementTemplateId,
            AchievementAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);

        return await SaveChangesAsync<UserAchievement>();
    }
}
