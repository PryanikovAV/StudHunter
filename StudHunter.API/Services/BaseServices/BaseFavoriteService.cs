using StudHunter.API.ModelsDto.FavoriteDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseFavoriteService(StudHunterDbContext context) : BaseService(context)
{
    protected static FavoriteDto MapToFavoriteDto(Favorite favorite)
    {
        return new FavoriteDto
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            VacancyId = favorite.VacancyId,
            EmployerId = favorite.EmployerId,
            StudentId = favorite.StudentId,
            AddedAt = favorite.AddedAt,
            Vacancy = favorite.Vacancy != null ? new VacancySummaryDto
            {
                Id = favorite.Vacancy.Id,
                Title = favorite.Vacancy.Title
            } : null,
            Employer = favorite.Employer != null ? new EmployerSummaryDto
            {
                Id = favorite.Employer.Id,
                Name = favorite.Employer.Name
            } : null,
            Student = favorite.Student != null ? new StudentSummaryDto
            {
                Id = favorite.Student.Id,
                FirstName = favorite.Student.FirstName,
                LastName = favorite.Student.LastName
            } : null
        };
    }
}
