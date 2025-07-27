using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class UserAchievementService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<(List<UserAchievementDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllUserAchievementsAsync(Guid userId)
    {
        var userAchievements = await _context.UserAchievements
        .Where(ua => ua.UserId == userId)
        .Include(ua => ua.AchievementTemplate)
        .Select(ua => new UserAchievementDto
        {
            Id = ua.UserId,
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

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> GrantAchievementAsync(Guid userId, int achievementTemplateOrderNumber)
    {
        #region Serializers
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (userExists == false)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("User"));

        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.OrderNumber == achievementTemplateOrderNumber);
        if (template == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("AchievementTemplate"));

        var achievementExists = await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementTemplateId == template.Id);
        if (achievementExists)
            return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Achievement", "UserId, AchievementTemplateOrderNumber"));
        #endregion

        var userAchievement = new UserAchievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AchievementTemplateId = template.Id,
            AchievementAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);

        var (success, statusCode, errorMessage) = await SaveChangesAsync("UserAchievement");

        return (success, statusCode, errorMessage);
    }

    //public async Task CheckAndGrantVacancyAchievementsAsync(Guid userId)
    //{
    //    int vacancyCount = await _context.Vacancies.CountAsync(v => v.EmployerId == userId && !v.IsDeleted);

    //    var userAchievements = await _context.UserAchievements
    //        .Where(ua => ua.UserId == userId)
    //        .Include(ua => ua.AchievementTemplate)
    //        .Select(ua => ua.AchievementTemplate.OrderNumber)
    //        .ToListAsync();

    //    if (vacancyCount >= 15 && !userAchievements.Contains(AchievementOrderNumbers.Vacancy15))
    //    {
    //        await GrantAchievementAsync(userId, AchievementOrderNumbers.Vacancy15);
    //    }
    //    else if (vacancyCount >= 10 && !userAchievements.Contains(AchievementOrderNumbers.Vacancy10))
    //    {
    //        await GrantAchievementAsync(userId, AchievementOrderNumbers.Vacancy10);
    //    }
    //    else if (vacancyCount >= 5 && !userAchievements.Contains(AchievementOrderNumbers.Vacancy5))
    //    {
    //        await GrantAchievementAsync(userId, AchievementOrderNumbers.Vacancy5);
    //    }
    //}

    //public async Task CheckAndGrantInvitationAchievementAsync(Guid userId)
    //{
    //    int invitationCount = await _context.Invitations.CountAsync(i => i.SenderId == userId);

    //    var userAchievements = await _context.UserAchievements
    //        .Where(ua => ua.UserId == userId)
    //        .Include(ua => ua.AchievementTemplate)
    //        .Select(ua => ua.AchievementTemplate.OrderNumber)
    //        .ToListAsync();

    //    if (invitationCount >= 10 && !userAchievements.Contains(AchievementOrderNumbers.Invitation10))
    //    {
    //        await GrantAchievementAsync(userId, AchievementOrderNumbers.Invitation10);
    //    }
    //}
}
