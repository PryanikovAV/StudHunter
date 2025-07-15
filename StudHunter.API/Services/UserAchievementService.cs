using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class UserAchievementService(StudHunterDbContext context) : BaseEntityService(context)
{
    public async Task<IEnumerable<UserAchievementDto>> GetUserAchievementsAsync(Guid userId)
    {
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.AchievementTemplate)
            .Select(ua => new UserAchievementDto
            {
                UserId = ua.UserId,
                AchievementTemplateId = ua.AchievementTemplateId,
                AchievementAt = ua.AchievementAt,
                AchievementName = ua.AchievementTemplate.Name,
                AchievementDescription = ua.AchievementTemplate.Description,
            })
            .OrderBy(ua => ua.AchievementAt)
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> GrantAchievementAync(Guid userId, int achievementTemplateId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return (false, "User not found");

        if (!await _context.AchievementTemplates.AnyAsync(a => a.Id == achievementTemplateId))
            return (false, "Achievement template not found");

        if (await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == achievementTemplateId))
            return (false, "User already has tgis achievement");

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementTemplateId = achievementTemplateId,
            AchievementAt = DateTime.Now
        };

        _context.UserAchievements.Add(userAchievement);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to grant achievement: {ex.InnerException?.Message}");
        }
        return (true, null);
    }
}
