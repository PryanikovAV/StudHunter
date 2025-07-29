using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminVacancyService(StudHunterDbContext context, UserAchievementService userAchievementService)
: VacancyService(context, userAchievementService)
{
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid id, AdminUpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        #region Serializers
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Vacancy"));
        #endregion

        if (dto.Title != null)
            vacancy.Title = dto.Title;
        if (dto.Description != null)
            vacancy.Description = dto.Description;
        if (dto.Salary.HasValue)
            vacancy.Salary = dto.Salary;
        vacancy.UpdatedAt = DateTime.UtcNow;
        if (dto.IsDeleted.HasValue)
            vacancy.IsDeleted = dto.IsDeleted.Value;
        if (dto.Type != null)
            vacancy.Type = Enum.Parse<Vacancy.VacancyType>(dto.Type);
        vacancy.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        return (success, statusCode, errorMessage);
    }

    public override async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid id, UpdateVacancyDto dto)
    {
        return await Task.FromException<(bool Success, int? StatusCode, string? ErrorMessage)>(
        new NotSupportedException("Admins must use AdminUpdateVacancyDto."));
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteVacancyAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Vacancy>(id, hardDelete);
    }
}
