using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record StudentProfileDto(
    Guid Id,
    string Email,

    [Required(ErrorMessage = "Имя обязательно")]
    string FirstName,

    [Required(ErrorMessage = "Фамилия обязательна")]
    string LastName,

    string? Patronymic,

    [Phone]
    string? ContactPhone,

    [EmailAddress]
    string? ContactEmail,

    Guid? CityId,
    string? Gender,
    DateOnly? BirthDate,
    string? AvatarUrl,
    bool IsForeign,
    string Status,
    Guid? UniversityId,
    Guid? FacultyId,
    Guid? DepartmentId,
    Guid? StudyDirectionId,

    [Range(1, 6)]
    int CourseNumber,
    string StudyForm,
    List<Guid> CourseIds,
    List<LookupDto> Courses
);

public static class StudentProfileMapper
{
    public static StudentProfileDto ToProfileDto(Student s)
    {
        var sp = s.StudyPlan;

        return new StudentProfileDto(
            Id: s.Id,
            Email: s.Email,
            FirstName: s.FirstName,
            LastName: s.LastName,
            Patronymic: s.Patronymic,
            ContactPhone: s.ContactPhone,
            ContactEmail: s.ContactEmail,
            CityId: s.CityId,
            Gender: s.Gender?.ToString(),
            BirthDate: s.BirthDate,
            AvatarUrl: s.AvatarUrl,
            IsForeign: s.IsForeign ?? false,
            Status: s.Status.ToString(),

            UniversityId: sp?.UniversityId,
            FacultyId: sp?.FacultyId,
            DepartmentId: sp?.DepartmentId,
            StudyDirectionId: sp?.StudyDirectionId,
            CourseNumber: sp?.CourseNumber ?? 1,
            StudyForm: sp?.StudyForm.ToString() ?? "FullTime",
            CourseIds: sp?.StudyPlanCourses?.Select(c => c.CourseId).ToList() ?? new List<Guid>(),
            Courses: sp?.StudyPlanCourses?.Select(c => new LookupDto(c.Course.Id, c.Course.Name)).ToList() ?? new List<LookupDto>()
        );
    }

    public static void ApplyProfileUpdate(Student student, StudentProfileDto dto, StudyPlan studyPlan)
    {
        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.Patronymic = dto.Patronymic?.Trim();
        student.ContactPhone = dto.ContactPhone;
        student.ContactEmail = dto.ContactEmail;
        student.CityId = dto.CityId;
        student.AvatarUrl = dto.AvatarUrl;
        student.BirthDate = dto.BirthDate;
        student.IsForeign = dto.IsForeign;

        if (Enum.TryParse<Student.StudentGender>(dto.Gender, out var gender))
            student.Gender = gender;

        if (Enum.TryParse<Student.StudentStatus>(dto.Status, out var status))
            student.Status = status;

        studyPlan.UniversityId = dto.UniversityId;
        studyPlan.FacultyId = dto.FacultyId;
        studyPlan.DepartmentId = dto.DepartmentId;
        studyPlan.StudyDirectionId = dto.StudyDirectionId;
        studyPlan.CourseNumber = dto.CourseNumber;

        if (Enum.TryParse<StudyPlan.StudyPlanForm>(dto.StudyForm, out var form))
            studyPlan.StudyForm = form;

        var dtoCourseIds = dto.CourseIds ?? new List<Guid>();

        var coursesToRemove = studyPlan.StudyPlanCourses
            .Where(spc => !dtoCourseIds.Contains(spc.CourseId))
            .ToList();

        foreach (var course in coursesToRemove)
        {
            studyPlan.StudyPlanCourses.Remove(course);
        }

        var existingCourseIds = studyPlan.StudyPlanCourses.Select(spc => spc.CourseId).ToHashSet();

        foreach (var courseId in dtoCourseIds)
        {
            if (!existingCourseIds.Contains(courseId))
            {
                studyPlan.StudyPlanCourses.Add(new StudyPlanCourse
                {
                    StudyPlanId = studyPlan.Id,
                    CourseId = courseId
                });
            }
        }
    }
}