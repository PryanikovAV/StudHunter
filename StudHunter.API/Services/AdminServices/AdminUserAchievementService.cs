using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminUserAchievementService(StudHunterDbContext context) : UserAchievementService(context)
{
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

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteUserAchievementAsync(Guid userId, int achievementTemplateOrderNumber)
    {
        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.OrderNumber == achievementTemplateOrderNumber);

        #region Serializers
        if (template == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("AchievementTemplate"));

        var userAchievement = await _context.UserAchievements.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == template.Id);
        if (userAchievement == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("UserAchievement"));
        #endregion

        return await DeleteEntityAsync<UserAchievement>(userAchievement.Id, hardDelete: true);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> GrantAchievementAsync(Guid userId, int achievementTemplateOrderNumber)
    {
        return await base.GrantAchievementAsync(userId, achievementTemplateOrderNumber);
    }
}
