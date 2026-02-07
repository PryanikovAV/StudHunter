using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record StudyPlanDto(
    Guid Id,
    Guid StudentId,
    Guid? UniversityId,
    string? UniversityName,
    string? UniversityAbbreviation,
    int CourseNumber,
    Guid? FacultyId,
    string? FacultyName,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? StudyDirectionId,
    string? StudyDirectionName,
    string? StudyDirectionCode,
    string StudyForm,
    List<string> CourseNames
);

public record UpdateStudyPlanDto(
    Guid? UniversityId,
    [Range(1, 6)] int? CourseNumber,
    Guid? FacultyId,
    Guid? DepartmentId,
    Guid? StudyDirectionId,
    [RegularExpression("FullTime|PartTime|Correspondence")] string? StudyForm);

public static class StudyPlanMapper
{
    public static StudyPlanDto ToDto(StudyPlan sp) => new(
        sp.Id,
        sp.StudentId,
        sp.UniversityId,
        sp.University?.Name,
        sp.University?.Abbreviation,
        sp.CourseNumber,
        sp.FacultyId,
        sp.Faculty?.Name,
        sp.DepartmentId,
        sp.Department?.Name,
        sp.StudyDirectionId,
        sp.StudyDirection?.Name,
        sp.StudyDirection?.Code,
        sp.StudyForm.ToString(),
        sp.StudyPlanCourses?
            .Select(spc => spc.Course.Name)
            .OrderBy(n => n)
            .ToList() ?? new List<string>()
    );

    public static void ApplyUpdate(StudyPlan studyPlan, UpdateStudyPlanDto dto)
    {
        if (dto.UniversityId.HasValue)
            studyPlan.UniversityId = dto.UniversityId.Value;
       
        if (dto.CourseNumber.HasValue)
            studyPlan.CourseNumber = dto.CourseNumber.Value;

        if (dto.FacultyId.HasValue)
            studyPlan.FacultyId = dto.FacultyId.Value;

        if (dto.StudyDirectionId.HasValue)
            studyPlan.StudyDirectionId = dto.StudyDirectionId.Value;

        if (dto.DepartmentId.HasValue)
            studyPlan.DepartmentId = dto.DepartmentId.Value;

        if (dto.StudyForm is not null && Enum.TryParse<StudyPlan.StudyPlanForm>(dto.StudyForm, out var form))
            studyPlan.StudyForm = form;
    }
}