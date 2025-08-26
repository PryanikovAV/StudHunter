using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseVacancyService(StudHunterDbContext context) : BaseService(context)
{
    protected TDto MapToVacancyDto<TDto>(Vacancy vacancy) where TDto : VacancyDto, new()
    {
        var dto = new TDto
        {
            Id = vacancy.Id,
            EmployerId = vacancy.EmployerId,
            Title = vacancy.Title,
            Description = vacancy.Description,
            Salary = vacancy.Salary,
            CreatedAt = vacancy.CreatedAt,
            UpdatedAt = vacancy.UpdatedAt,
            Type = vacancy.Type.ToString()
        };

        if (dto is AdminVacancyDto adminVacancyDto)
        {
            adminVacancyDto.IsDeleted = vacancy.IsDeleted;
        }

        return dto;
    }
}
