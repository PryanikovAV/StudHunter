using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

public class AdminUserAchievementService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<IEnumerable<UserAchievementDto>> GetAllUserAchievementsAsync()
    {
        return await _context.UserAchievements
        .Include(ua => ua.AchievementTemplate)
        .Select(ua => new UserAchievementDto
        {
            UserId = ua.UserId,
            AchievementTemplateId = ua.AchievementTemplateId,
            AchievementAt = ua.AchievementAt,
            AchievementName = ua.AchievementTemplate.Name,
            AchievementDescription = ua.AchievementTemplate.Description
        })
        .OrderBy(ua => ua.AchievementAt)
        .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> GrantAchievementAsync(Guid userId, int achievementTemplateId)
    {
        return await _userAchievementService.GrantAchievementAsync(userId, achievementTemplateId);
    }

    public async Task<(bool Success, string? Error)> DeleteUserAchievementAsync(Guid userId, int achievementTemplateId)
    {
        var userAchievement = await _context.UserAchievements.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == achievementTemplateId);
        if (userAchievement == null)
            return (false, "User achievement not found");

        _context.UserAchievements.Remove(userAchievement);

        return await SaveChangesAsync("delete", "UserAchievement");
    }
}
