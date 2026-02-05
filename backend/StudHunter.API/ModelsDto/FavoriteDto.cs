using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto;

public enum FavoriteType { Vacancy, Resume, Employer }

public record FavoriteDto(
    Guid Id,
    DateTime AddedAt,
    FavoriteType Type,
    FavoriteVacancyDto? Vacancy = null,
    FavoriteResumeDto? Resume = null,
    FavoriteEmployerDto? Employer = null
);

public record FavoriteVacancyDto(
    Guid Id,
    string Title,
    string CompanyName,
    decimal? Salary
);

public record FavoriteResumeDto(
    Guid Id,
    string Title,
    string StudentName,
    string? AvatarUrl
);

public record FavoriteEmployerDto(
    Guid Id,
    string Name,
    string? AvatarUrl
);

public static class FavoriteMapper
{
    public static FavoriteDto ToDto(Favorite favorite)
    {
        if (favorite.VacancyId.HasValue && favorite.Vacancy != null)
        {
            return new FavoriteDto(
                favorite.Id,
                favorite.AddedAt,
                FavoriteType.Vacancy,
                Vacancy: new FavoriteVacancyDto(
                    favorite.Vacancy.Id,
                    favorite.Vacancy.Title,
                    favorite.Vacancy.Employer?.Name ?? UserDefaultNames.DefaultCompanyName,
                    favorite.Vacancy.Salary)
            );
        }

        if (favorite.ResumeId.HasValue && favorite.Resume != null)
        {
            var student = favorite.Resume.Student;
            return new FavoriteDto(
                favorite.Id,
                favorite.AddedAt,
                FavoriteType.Resume,
                Resume: new FavoriteResumeDto(
                    favorite.Resume.Id,
                    favorite.Resume.Title,
                    student != null ? $"{student.LastName} {student.FirstName}".Trim()
                    : $"{UserDefaultNames.DefaultLastName} {UserDefaultNames.DefaultFirstName}",
                    student?.AvatarUrl)
            );
        }

        if (favorite.EmployerId.HasValue && favorite.Employer != null)
        {
            return new FavoriteDto(
                favorite.Id,
                favorite.AddedAt,
                FavoriteType.Employer,
                Employer: new FavoriteEmployerDto(
                    favorite.Employer.Id,
                    favorite.Employer.Name,
                    favorite.Employer.AvatarUrl)
            );
        }

        throw new InvalidOperationException("Объект избранного не содержит данных.");
    }
}