using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record StudentDto(
    Guid Id,
    string Email,
    string RegistrationStage,
    string FirstName,
    string LastName,
    string? Patronymic,
    string? ContactPhone,
    string? ContactEmail,
    Guid? CityId,
    string? CityName,
    string? Gender,
    DateOnly? BirthDate,
    string? AvatarUrl,
    bool IsForeign,
    string Status,

    Guid? UniversityId,
    string? UniversityName,
    Guid? FacultyId,
    string? FacultyName,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? StudyDirectionId,
    string? StudyDirectionName,
    int CourseNumber,
    string StudyForm,
    List<Guid> CourseIds,
    List<LookupDto> Courses,

    DateTime CreatedAt,
    Guid? ResumeId
);

public record AdminStudentDto(
    Guid Id,
    string Email,
    string RegistrationStage,
    string FirstName,
    string LastName,
    string? Patronymic,
    string? ContactPhone,
    string? ContactEmail,
    Guid? CityId,
    string? CityName,
    string? Gender,
    DateOnly? BirthDate,
    string? AvatarUrl,
    bool IsForeign,
    string Status,
     Guid? UniversityId,
    string? UniversityName,
    Guid? FacultyId,
    string? FacultyName,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? StudyDirectionId,
    string? StudyDirectionName,
    int CourseNumber,
    string StudyForm,
    List<Guid> CourseIds,
    List<LookupDto> Courses,
    DateTime CreatedAt,
    Guid? ResumeId,
    bool IsDeleted)
    : StudentDto(Id, Email, RegistrationStage, FirstName, LastName, Patronymic, ContactPhone, ContactEmail,
        CityId, CityName, Gender, BirthDate, AvatarUrl, IsForeign, Status, UniversityId, UniversityName,
        FacultyId, FacultyName, DepartmentId, DepartmentName, StudyDirectionId, StudyDirectionName, CourseNumber,
        StudyForm, CourseIds, Courses, CreatedAt, ResumeId);

public record StudentHeroDto(
    Guid Id,
    string FullName,
    DateOnly? BirthDate,
    string? AvatarUrl,
    string Status,
    string? UniversityName,
    string? FacultyName,
    string? DepartmentName,
    string? StudyDirectionName,
    int? CourseNumber
);

public record UpdateStudentDto(
    [Required][StringLength(50)] string FirstName,
    [Required][StringLength(50)] string LastName,
    [StringLength(50)] string? Patronymic,
    [Phone] string? ContactPhone,
    [EmailAddress] string? ContactEmail,
    Guid? CityId,
    [RegularExpression("Male|Female")] string? Gender,
    DateOnly? BirthDate,
    bool IsForeign,
    [RegularExpression("Studying|SeekingInternship|SeekingJob|Interning|Working")] string? Status,

    Guid? UniversityId,
    Guid? FacultyId,
    Guid? DepartmentId,
    Guid? StudyDirectionId,
    [Range(1, 6)] int CourseNumber,
    string StudyForm,
    List<Guid>? CourseIds
);

public static class StudentMapper
{
    public static StudentDto ToDto(Student s)
    {
        var sp = s.StudyPlan;
        return new StudentDto(
            s.Id,
            s.Email,
            s.RegistrationStage.ToString(),
            s.FirstName,
            s.LastName,
            s.Patronymic,
            s.ContactPhone,
            s.ContactEmail,
            s.CityId,
            s.City?.Name,
            s.Gender?.ToString(),
            s.BirthDate,
            s.AvatarUrl,
            s.IsForeign ?? false, s.Status.ToString(),
            sp?.UniversityId,
            sp?.University?.Name,
            sp?.FacultyId,
            sp?.Faculty?.Name,
            sp?.DepartmentId,
            sp?.Department?.Name,
            sp?.StudyDirectionId,
            sp?.StudyDirection?.Name,
            sp?.CourseNumber ?? 1,
            sp?.StudyForm.ToString() ?? "FullTime",
            sp?.StudyPlanCourses?.Select(c => c.CourseId).ToList() ?? new List<Guid>(),
            sp?.StudyPlanCourses?.Select(c => new LookupDto(c.Course.Id, c.Course.Name)).ToList() ?? new List<LookupDto>(),
            s.CreatedAt,
            s.Resume?.Id
        );
    }

    public static AdminStudentDto ToAdminDto(Student s)
    {
        var sp = s.StudyPlan;
        return new AdminStudentDto(
            s.Id,
            s.Email,
            s.RegistrationStage.ToString(),
            s.FirstName,
            s.LastName,
            s.Patronymic,
            s.ContactPhone,
            s.ContactEmail,
            s.CityId,
            s.City?.Name,
            s.Gender?.ToString(),
            s.BirthDate,
            s.AvatarUrl,
            s.IsForeign ?? false,
            s.Status.ToString(),
            sp?.UniversityId,
            sp?.University?.Name,
            sp?.FacultyId,
            sp?.Faculty?.Name,
            sp?.DepartmentId,
            sp?.Department?.Name,
            sp?.StudyDirectionId,
            sp?.StudyDirection?.Name,
            sp?.CourseNumber ?? 1,
            sp?.StudyForm.ToString() ?? "FullTime",
            sp?.StudyPlanCourses?.Select(c => c.CourseId).ToList() ?? new List<Guid>(),
            sp?.StudyPlanCourses?.Select(c => new LookupDto(c.Course.Id, c.Course.Name)).ToList() ?? new List<LookupDto>(),
            s.CreatedAt, s.Resume?.Id,
            s.IsDeleted
        );
    }

    public static StudentHeroDto ToHeroDto(Student s) => new(
        Id: s.Id,
        FullName: $"{s.LastName} {s.FirstName} {s.Patronymic}".Trim(),
        BirthDate: s.BirthDate,
        AvatarUrl: s.AvatarUrl,
        Status: s.Status.ToString(),
        UniversityName: s.StudyPlan?.University?.Name,
        FacultyName: s.StudyPlan?.Faculty?.Name,
        DepartmentName: s.StudyPlan?.Department?.Name,
        StudyDirectionName: s.StudyPlan?.StudyDirection?.Name,
        CourseNumber: s.StudyPlan?.CourseNumber
    );

    public static void ApplyUpdate(Student student, UpdateStudentDto dto)
    {
        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.Patronymic = dto.Patronymic?.Trim();
        student.ContactPhone = dto.ContactPhone?.Trim();
        student.ContactEmail = dto.ContactEmail?.Trim();
        student.CityId = dto.CityId;
        student.BirthDate = dto.BirthDate;
        student.IsForeign = dto.IsForeign;

        if (Enum.TryParse<Student.StudentGender>(dto.Gender, out var gender)) student.Gender = gender;
        if (Enum.TryParse<Student.StudentStatus>(dto.Status, out var status)) student.Status = status;

        student.StudyPlan ??= new StudyPlan { StudentId = student.Id, StudyPlanCourses = new List<StudyPlanCourse>() };

        var sp = student.StudyPlan;
        if (sp.IsDeleted)
        {
            sp.IsDeleted = false;
            sp.DeletedAt = null;
        }

        sp.UniversityId = dto.UniversityId;
        sp.FacultyId = dto.FacultyId;
        sp.DepartmentId = dto.DepartmentId;
        sp.StudyDirectionId = dto.StudyDirectionId;
        sp.CourseNumber = dto.CourseNumber;

        if (Enum.TryParse<StudyPlan.StudyPlanForm>(dto.StudyForm, out var form)) sp.StudyForm = form;

        var dtoCourseIds = dto.CourseIds ?? new List<Guid>();
        var coursesToRemove = sp.StudyPlanCourses.Where(spc => !dtoCourseIds.Contains(spc.CourseId)).ToList();
        foreach (var c in coursesToRemove) sp.StudyPlanCourses.Remove(c);

        var existingCourseIds = sp.StudyPlanCourses.Select(spc => spc.CourseId).ToHashSet();
        foreach (var courseId in dtoCourseIds)
        {
            if (!existingCourseIds.Contains(courseId))
                sp.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = sp.Id, CourseId = courseId });
        }
    }
}