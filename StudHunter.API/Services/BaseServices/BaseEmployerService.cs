using StudHunter.API.ModelsDto.EmployerDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseEmployerService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Maps an employer entity to a specified DTO type.
    /// </summary>
    /// <typeparam name="TDto">The type of DTO tp map to (must inherit from EmployerDto).</typeparam>
    /// <param name="employer">The employer entity to map.</param>
    /// <returns>The mapped DTO.</returns>
    protected TDto MapToEmployerDto<TDto>(Employer employer) where TDto : EmployerDto, new()
    {
        var dto = new TDto
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
            Achievements = employer.Achievements.Select(BaseUserAchievementService.MapToUserAchievementDto).ToList()
        };

        if (dto is AdminEmployerDto adminDto)
        {
            adminDto.IsDeleted = employer.IsDeleted;
        }

        return dto;
    }
}
