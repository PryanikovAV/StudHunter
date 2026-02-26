using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record ResumeFillDto(
    Guid? Id,
    [Required(ErrorMessage = "Заголовок резюме обязателен")] string Title,
    string? Description,
    bool IsDeleted,
    List<Guid> SkillIds,
    List<LookupDto>? Skills = null
);

public record ResumeSearchDto(
    Guid Id,
    Guid StudentId,
    string Title,
    string? Description,
    DateTime UpdatedAt,
    string? Email,
    string? Phone,
    string FullName,
    string? FacultyName,
    string? StudyDirectionName,
    int? CourseNumber,
    List<string> Skills,
    bool IsFavorite = false,
    bool IsBlocked = false
);

public record ResumeSearchFilter
{
    public string? SearchTerm { get; init; }
    public List<Guid> SkillIds { get; init; } = new();
    public PaginationParams Paging { get; init; } = new PaginationParams();
}

public record CreateOfferRequest(
    Guid? VacancyId,
    string? Message
    );

public static class ResumeMapper
{
    public static ResumeFillDto ToFillDto(Resume? resume)
    {
        if (resume == null)
            return new ResumeFillDto(null, string.Empty, null, false, new List<Guid>(), new List<LookupDto>());

        return new ResumeFillDto(
            Id: resume.Id,
            Title: resume.Title,
            Description: resume.Description,
            IsDeleted: resume.IsDeleted,
            SkillIds: resume.AdditionalSkills?.Select(s => s.AdditionalSkillId).ToList() ?? new List<Guid>(),
            Skills: resume.AdditionalSkills?.Select(s => new LookupDto(s.AdditionalSkill.Id, s.AdditionalSkill.Name)).ToList() ?? new List<LookupDto>()
        );
    }

    public static void ApplyUpdate(Resume resume, ResumeFillDto dto)
    {
        resume.Title = dto.Title.Trim();
        resume.Description = dto.Description?.Trim();
        resume.UpdatedAt = DateTime.UtcNow;

        var dtoSkillIds = dto.SkillIds ?? new List<Guid>();

        var skillsToRemove = resume.AdditionalSkills
            .Where(ras => !dtoSkillIds.Contains(ras.AdditionalSkillId))
            .ToList();

        foreach (var skill in skillsToRemove)
            resume.AdditionalSkills.Remove(skill);

        var existingSkillIds = resume.AdditionalSkills.Select(ras => ras.AdditionalSkillId).ToHashSet();

        foreach (var skillId in dtoSkillIds)
        {
            if (!existingSkillIds.Contains(skillId))
            {
                resume.AdditionalSkills.Add(new ResumeAdditionalSkill
                {
                    ResumeId = resume.Id,
                    AdditionalSkillId = skillId
                });
            }
        }
    }

    public static ResumeSearchDto ToSearchDto(Resume r, bool maskContacts, bool isFavorite = false, bool isBlocked = false)
    {
        var fullName = $"{r.Student.LastName} {r.Student.FirstName}".Trim();

        string? displayEmail = maskContacts ? "Доступно после аккредитации" : r.Student.ContactEmail ?? r.Student.Email;
        string? displayPhone = maskContacts ? "Скрыто" : r.Student.ContactPhone;

        var sp = r.Student.StudyPlan;

        return new ResumeSearchDto(
            Id: r.Id,
            StudentId: r.StudentId,
            Title: r.Title,
            Description: r.Description,
            UpdatedAt: r.UpdatedAt,
            Email: displayEmail,
            Phone: displayPhone,
            FullName: fullName,
            FacultyName: sp?.Faculty?.Name,
            StudyDirectionName: sp?.StudyDirection?.Name,
            CourseNumber: sp?.CourseNumber,
            Skills: r.AdditionalSkills?
                        .Select(ras => ras.AdditionalSkill.Name)
                        .OrderBy(n => n)
                        .ToList() ?? new List<string>(),
            IsFavorite: isFavorite,
            IsBlocked: isBlocked
        );
    }
}