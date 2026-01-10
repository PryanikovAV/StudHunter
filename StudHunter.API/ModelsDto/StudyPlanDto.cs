using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record StudyPlanDto(
    Guid Id,
    Guid StudentId,
    int CourseNumber,
    Guid? FacultyId,  // TODO: notnullable после тестов
    string? FacultyName,
    Guid? SpecialityId,  // TODO: notnullable после тестов
    string? SpecialityName,
    string StudyForm,
    List<string> CourseNames
);

public record UpdateStudyPlanDto(
    [Range(1, 6)] int? CourseNumber,
    Guid? FacultyId,
    Guid? SpecialityId,
    [RegularExpression("FullTime|PartTime|Correspondence")] string? StudyForm);

public static class StudyPlanMapper
{
    public static StudyPlanDto ToDto(StudyPlan sp) => new(
        sp.Id,
        sp.StudentId,
        sp.CourseNumber,
        sp.FacultyId,
        sp.Faculty?.Name,
        sp.SpecialityId,
        sp.Speciality?.Name,
        sp.StudyForm.ToString(),
        sp.StudyPlanCourses?
            .Select(spc => spc.Course.Name)
            .OrderBy(name => name)
            .ToList() ?? new List<string>()
    );

    public static void ApplyUpdate(StudyPlan studyPlan, UpdateStudyPlanDto dto)
    {
        if (dto.CourseNumber.HasValue)
            studyPlan.CourseNumber = dto.CourseNumber.Value;

        if (dto.FacultyId.HasValue)
            studyPlan.FacultyId = dto.FacultyId.Value;

        if (dto.SpecialityId.HasValue)
            studyPlan.SpecialityId = dto.SpecialityId.Value;

        if (dto.StudyForm is not null && Enum.TryParse<StudyPlan.StudyPlanForm>(dto.StudyForm, out var form))
            studyPlan.StudyForm = form;
    }
}