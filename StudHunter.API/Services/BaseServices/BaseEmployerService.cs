using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public class BaseEmployerService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

    /// <summary>
    /// Retrieves an employer by their ID, including related vacancies and achievements.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync(Guid id)
    {
        var employer = await _context.Employers
        .Include(e => e.Vacancies)
        .Include(s => s.Achievements)
        .ThenInclude(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(e => e.Id == id);

        #region Serializers
        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Employer)));
        #endregion

        return (new EmployerDto
        {
            Id = employer.Id,
            Email = employer.Email,
            ContactEmail = employer.ContactEmail,
            ContactPhone = employer.ContactPhone,
            CreatedAt = employer.CreatedAt,
            AccreditationStatus = employer.AccreditationStatus,
            Name = employer.Name,
            Description = employer.Description,
            Website = employer.Website,
            Specialization = employer.Specialization,
            VacancyIds = employer.Vacancies.Select(v => v.Id).ToList(),
            Achievements = employer.Achievements.Select(userAchievement => new UserAchievementDto
            {
                Id = userAchievement.Id,
                UserId = userAchievement.UserId,
                AchievementTemplateOrderNumber = userAchievement.AchievementTemplate.OrderNumber,
                AchievementAt = userAchievement.AchievementAt,
                AchievementName = userAchievement.AchievementTemplate.Name,
                AchievementDescription = userAchievement.AchievementTemplate.Description
            }).ToList()
        }, null, null);
    }
}
