using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.UserAchievement;
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

    public async Task<(UserAchievementDto? Entity, int? StatusCode, string? ErrorMessage)> GetUserAchievementAsync(Guid userId, int orderNumber)
    {
        var userAchievement = await _context.UserAchievements
        .Include(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplate.OrderNumber == orderNumber);

        #region Serializers
        if (userAchievement == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("UserAchievement"));
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

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteUserAchievementAsync(Guid userId, int orderNumber)
    {
        var userAchievement = await _context.UserAchievements
        .Include(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementTemplate.OrderNumber == orderNumber);

        #region Serializers
        if (userAchievement == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("UserAchievement"));
        #endregion

        return await DeleteEntityAsync<UserAchievement>(userAchievement.Id, hardDelete: true);
    }
}
