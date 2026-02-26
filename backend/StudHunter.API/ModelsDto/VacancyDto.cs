using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record VacancyFillDto(
    Guid? Id,
    [Required(ErrorMessage = "Название вакансии обязательно")] string Title,
    string? Description,
    decimal? Salary,
    [Required] string Type,
    List<Guid> CourseIds,
    List<Guid> SkillIds,
    List<LookupDto>? Courses = null,
    List<LookupDto>? Skills = null
);

public record VacancySearchDto(
    Guid Id,
    Guid EmployerId,
    string Title,
    string? Description,
    decimal? Salary,
    string Type,
    DateTime UpdatedAt,
    string EmployerName,
    string? SpecializationName,
    string? CityName,
    string? ActualAddress,
    string? ContactPhone,
    string? ContactEmail,
    bool IsDeleted,
    List<string> Courses,
    List<string> Skills,
    bool IsFavorite = false,
    bool IsBlocked = false
);

public record VacancySearchFilter
{
    public Guid? EmployerId { get; init; }
    public string? SearchTerm { get; init; }
    public List<Guid> CourseIds { get; init; } = new();
    public List<Guid> SkillIds { get; init; } = new();
    public string? VacancyType { get; init; }
    public PaginationParams Paging { get; init; } = new PaginationParams();
}

public static class VacancyMapper
{
    public static VacancyFillDto ToFillDto(Vacancy v) => new(
        Id: v.Id,
        Title: v.Title,
        Description: v.Description,
        Salary: v.Salary,
        Type: v.Type.ToString(),
        CourseIds: v.Courses?.Select(c => c.CourseId).ToList() ?? new List<Guid>(),
        SkillIds: v.AdditionalSkills?.Select(s => s.AdditionalSkillId).ToList() ?? new List<Guid>(),
        Courses: v.Courses?.Select(c => new LookupDto(c.Course.Id, c.Course.Name)).ToList(),
        Skills: v.AdditionalSkills?.Select(s => new LookupDto(s.AdditionalSkill.Id, s.AdditionalSkill.Name)).ToList()
    );

    public static VacancySearchDto ToSearchDto(Vacancy v, bool isFavorite = false, bool isBlocked = false)
    {
        var e = v.Employer;
        return new VacancySearchDto(
            Id: v.Id,
            EmployerId: v.EmployerId,
            Title: v.Title,
            Description: v.Description,
            Salary: v.Salary,
            Type: v.Type.ToString(),
            UpdatedAt: v.UpdatedAt,
            EmployerName: e.Name,
            SpecializationName: e.Specialization?.Name,
            CityName: e.City?.Name,
            ActualAddress: e.OrganizationDetails?.ActualAddress,
            ContactPhone: e.ContactPhone,
            ContactEmail: e.ContactEmail ?? e.Email,
            IsDeleted: v.IsDeleted,
            Courses: v.Courses?.Select(c => c.Course.Name).OrderBy(n => n).ToList() ?? new List<string>(),
            Skills: v.AdditionalSkills?.Select(s => s.AdditionalSkill.Name).OrderBy(n => n).ToList() ?? new List<string>(),
            IsFavorite: isFavorite,
            IsBlocked: isBlocked
        );
    }

    public static void ApplyUpdate(Vacancy vacancy, VacancyFillDto dto)
    {
        vacancy.Title = dto.Title.Trim();
        vacancy.Description = dto.Description?.Trim();
        vacancy.Salary = dto.Salary;
        vacancy.UpdatedAt = DateTime.UtcNow;

        if (Enum.TryParse<Vacancy.VacancyType>(dto.Type, out var type))
            vacancy.Type = type;

        var dtoCourseIds = dto.CourseIds ?? new List<Guid>();
        var coursesToRemove = vacancy.Courses.Where(c => !dtoCourseIds.Contains(c.CourseId)).ToList();
        foreach (var c in coursesToRemove) vacancy.Courses.Remove(c);

        var existingCourseIds = vacancy.Courses.Select(c => c.CourseId).ToHashSet();
        foreach (var id in dtoCourseIds)
        {
            if (!existingCourseIds.Contains(id))
                vacancy.Courses.Add(new VacancyCourse { VacancyId = vacancy.Id, CourseId = id });
        }

        var dtoSkillIds = dto.SkillIds ?? new List<Guid>();
        var skillsToRemove = vacancy.AdditionalSkills.Where(s => !dtoSkillIds.Contains(s.AdditionalSkillId)).ToList();
        foreach (var s in skillsToRemove) vacancy.AdditionalSkills.Remove(s);

        var existingSkillIds = vacancy.AdditionalSkills.Select(s => s.AdditionalSkillId).ToHashSet();
        foreach (var id in dtoSkillIds)
        {
            if (!existingSkillIds.Contains(id))
                vacancy.AdditionalSkills.Add(new VacancyAdditionalSkill { VacancyId = vacancy.Id, AdditionalSkillId = id });
        }
    }
}