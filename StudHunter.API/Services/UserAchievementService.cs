using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class UserAchievementService(StudHunterDbContext context) : BaseService(context)
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

    public async Task<(bool Success, string? Error)> GrantAchievementAsync(Guid userId, int achievementTemplateId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return (false, "User not found");

        if (!await _context.AchievementTemplates.AnyAsync(a => a.Id == achievementTemplateId))
            return (false, "Achievement template not found");

        if (await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == achievementTemplateId))
            return (false, "User already has this achievement");

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementTemplateId = achievementTemplateId,
            AchievementAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);

        return await SaveChangesAsync("grant achievement", "UserAchievement");
    }

    // ===== Achievements =====
    // TODO: Check and fix all Id's and count of achievement elements
    public async Task CheckAndGrantVacancyAchievementsAsync(Guid userId)
    {
        int vacancyCount = await _context.Vacancies.CountAsync(v => v.EmployerId == userId && !v.IsDeleted);

        var userAchievements = await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementTemplateId)
            .ToListAsync();

        if (vacancyCount >= 15 && !userAchievements.Contains(3))
        {
            await GrantAchievementAsync(userId, 3);
        }
        else if (vacancyCount >= 10 && !userAchievements.Contains(2))
        {
            await GrantAchievementAsync(userId, 2);
        }
        else if (vacancyCount >= 5 && !userAchievements.Contains(1))
        {
            await GrantAchievementAsync(userId, 1);
        }
    }

    public async Task CheckAndGrantInvitationAchievementAsync(Guid userId)
    {
        int invitationCount = await _context.Invitations.CountAsync(i => i.SenderId == userId);

        var userAchievements = await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementTemplateId)
            .ToListAsync();

        if (invitationCount >= 10 && !userAchievements.Contains(2))
        {
            await GrantAchievementAsync(userId, 2);
        }
    }
    // ===== Achievements =====
}
