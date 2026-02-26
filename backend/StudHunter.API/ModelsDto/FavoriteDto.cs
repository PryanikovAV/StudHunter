using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto;

public enum FavoriteType { Vacancy, Student, Employer }

public record FavoriteCardDto(
    Guid Id,
    Guid TargetId,
    string Type,
    string Title,
    string? Subtitle,
    string? AvatarUrl,
    DateTime AddedAt
);

public record FavoriteRequest(
    Guid TargetId,
    FavoriteType Type,
    bool IsFavorite
);

public static class FavoriteMapper
{
    public static FavoriteCardDto ToCardDto(Favorite f)
    {
        if (f.VacancyId.HasValue && f.Vacancy != null)
        {
            return new FavoriteCardDto(
                Id: f.Id,
                TargetId: f.VacancyId.Value,
                Type: FavoriteType.Vacancy.ToString(),
                Title: f.Vacancy.Title,
                Subtitle: f.Vacancy.Salary.HasValue ? $"{f.Vacancy.Salary} руб." : "Зарплата не указана",
                AvatarUrl: f.Vacancy.Employer?.AvatarUrl,
                AddedAt: f.AddedAt
            );
        }

        if (f.StudentId.HasValue && f.Student != null)
        {
            var s = f.Student;
            var uni = s.StudyPlan?.University?.Abbreviation;
            var course = s.StudyPlan?.CourseNumber;
            var subtitle = (uni != null && course != null) ? $"{uni}, {course} курс" : "Студент";

            return new FavoriteCardDto(
                Id: f.Id,
                TargetId: f.StudentId.Value,
                Type: FavoriteType.Student.ToString(),
                Title: UserDisplayHelper.GetUserDisplayName(s),
                Subtitle: subtitle,
                AvatarUrl: s.AvatarUrl,
                AddedAt: f.AddedAt
            );
        }

        if (f.EmployerId.HasValue && f.Employer != null)
        {
            return new FavoriteCardDto(
                Id: f.Id,
                TargetId: f.EmployerId.Value,
                Type: FavoriteType.Employer.ToString(),
                Title: UserDisplayHelper.GetUserDisplayName(f.Employer),
                Subtitle: "Компания",
                AvatarUrl: f.Employer.AvatarUrl,
                AddedAt: f.AddedAt
            );
        }

        throw new InvalidOperationException("Объект избранного не содержит данных.");
    }
}