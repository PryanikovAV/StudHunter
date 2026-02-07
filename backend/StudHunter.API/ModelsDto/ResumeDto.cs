using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record ResumeDto(
    Guid Id,
    Guid StudentId,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,

    string? Email,
    string? Phone,

    string FullName,
    string? FacultyName,
    string? SpecialityName,
    int? CourseNumber,
    bool IsCommunicationBlocked,
    List<string> Skills
);

public record UpdateResumeDto(
    [StringLength(255)] string? Title,
    [StringLength(5000)] string? Description,
    List<Guid>? SkillIds
);

public static class ResumeMapper
{
    public static ResumeDto ToDto(Resume r, bool isBlocked = false, bool maskContacts = false)
    {
        var fullName = $"{r.Student.LastName} {r.Student.FirstName}".Trim();

        string? displayEmail = maskContacts ? "Доступно после аккредитации" : r.Student.Email;
        string? displayPhone = maskContacts ? "Скрыто" : r.Student.ContactPhone;

        return new ResumeDto(
            r.Id,
            r.StudentId,
            r.Title,
            r.Description,
            r.CreatedAt,
            r.UpdatedAt,
            displayEmail,
            displayPhone,
            fullName,
            r.Student.StudyPlan?.Faculty?.Name,
            r.Student.StudyPlan?.StudyDirection?.Name,
            r.Student.StudyPlan?.CourseNumber,
            isBlocked,
            r.AdditionalSkills?.Select(ras => ras.AdditionalSkill.Name)
                               .OrderBy(n => n)
                               .ToList() ?? new List<string>()
        );
    }

    public static void ApplyUpdate(Resume resume, UpdateResumeDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Title))
            resume.Title = dto.Title.Trim();

        if (dto.Description != null)
            resume.Description = dto.Description.Trim();

        resume.UpdatedAt = DateTime.UtcNow;
    }
}