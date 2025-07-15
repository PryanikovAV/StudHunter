using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdministratorServices;

public class AdministratorUserAchievementService(StudHunterDbContext context) : BaseAdministratorService(context)
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

    public async Task<(bool Success, string? Error)> DeleteUserAchievementAsync(Guid userId, int achievementTemplateId)
    {
        var userAchievement = await _context.UserAchievements.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == achievementTemplateId);
        if (userAchievement == null)
            return (false, "User achievement not found");

        _context.UserAchievements.Remove(userAchievement);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete user achievement: {ex.InnerException?.Message}");
        }
        return (true, null);
    }
}
