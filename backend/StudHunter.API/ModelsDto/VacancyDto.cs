using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record VacancyDto(
    Guid Id,
    Guid EmployerId,
    string Title,
    string? Description,
    decimal? Salary,
    string Type,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsDeleted,

    string EmployerName,
    bool IsEmployerVerified,

    bool IsCommunicationBlocked,
    bool CanApply,

    List<string> RequiredSkills,
    List<string> TargetCourses
);

public record UpdateVacancyDto(
    [StringLength(255)] string? Title,
    [StringLength(2500)] string? Description,
    [Range(0, 1000000)] decimal? Salary,
    Vacancy.VacancyType? Type,
    List<Guid>? SkillIds,
    List<Guid>? CourseIds
);

public static class VacancyMapper
{
    public static VacancyDto ToDto(Vacancy v, bool isBlocked = false, User.AccountStatus studentStage = User.AccountStatus.Anonymous)
    {
        // Студент может откликнуться, если:
        // 1. Вакансия не удалена
        // 2. Нет блокировок в ЧС
        // 3. Его Stage >= Stage 2
        bool canApply = !v.IsDeleted &&
                        !isBlocked &&
                        studentStage >= User.AccountStatus.ProfileFilled;

        return new VacancyDto(
            v.Id,
            v.EmployerId,
            v.Title,
            v.Description,
            v.Salary,
            v.Type.ToString(),
            v.CreatedAt,
            v.UpdatedAt,
            v.IsDeleted,
            v.Employer.Name,
            v.Employer.RegistrationStage == User.AccountStatus.FullyActivated,
            isBlocked,
            canApply,
            v.AdditionalSkills?.Select(vas => vas.AdditionalSkill.Name).OrderBy(n => n).ToList() ?? new List<string>(),
            v.Courses?.Select(vc => vc.Course.Name).OrderBy(n => n).ToList() ?? new List<string>()
        );
    }

    public static void ApplyUpdate(Vacancy vacancy, UpdateVacancyDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Title))
            vacancy.Title = dto.Title.Trim();
        if (dto.Description != null) vacancy.Description = dto.Description.Trim();
        if (dto.Salary.HasValue) vacancy.Salary = dto.Salary.Value;
        if (dto.Type.HasValue) vacancy.Type = dto.Type.Value;
    }
}