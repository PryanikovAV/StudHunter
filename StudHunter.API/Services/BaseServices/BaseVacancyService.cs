using StudHunter.API.ModelsDto.VacancyDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseVacancyService(StudHunterDbContext context) : BaseService(context)
{
    protected VacancyDto MapToVacancyDto(Vacancy vacancy)
    {
        return new VacancyDto
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
    }
}
