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
    bool? IsForeign,
    string Status,
    DateTime CreatedAt,
    Guid? ResumeId);

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
    bool? IsForeign,
    string Status,
    DateTime CreatedAt,
    Guid? ResumeId,
    bool IsDeleted)
    : StudentDto(Id, Email, RegistrationStage, FirstName, LastName, Patronymic, ContactPhone, ContactEmail,
        CityId, CityName, Gender, BirthDate, AvatarUrl, IsForeign, Status, CreatedAt, ResumeId);

public record StudentHeroDto(
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
    [StringLength(50)] string? FirstName,
    [StringLength(50)] string? LastName,
    [StringLength(50)] string? Patronymic,
    [Phone] string? ContactPhone,
    [StringLength(255)] string? AvatarUrl,
    [EmailAddress] string? ContactEmail,
    Guid? CityId,
    [RegularExpression("Male|Female")] string? Gender,
    DateOnly? BirthDate,
    bool? IsForeign,
    [RegularExpression("Studying|SeekingInternship|SeekingJob|Interning|Working")] string? Status,
    [StringLength(255, MinimumLength = 8)] string? Password);

public static class StudentMapper
{
    public static StudentDto ToDto(Student s) => new(
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
        s.IsForeign,
        s.Status.ToString(),
        s.CreatedAt,
        s.Resume?.Id);

    public static AdminStudentDto ToAdminDto(Student s) => new(
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
        s.IsForeign,
        s.Status.ToString(),
        s.CreatedAt,
        s.Resume?.Id,
        s.IsDeleted);

    public static StudentHeroDto ToHeroDto(Student student, StudyPlanDto? studyPlan) => new(
        FullName: $"{student.LastName} {student.FirstName} {student.Patronymic}".Trim(),
        BirthDate: student.BirthDate,
        AvatarUrl: student.AvatarUrl,
        Status: student.Status.ToString(),

        UniversityName: studyPlan?.UniversityName,
        FacultyName: studyPlan?.FacultyName,
        DepartmentName: studyPlan?.DepartmentName,
        StudyDirectionName: studyPlan?.StudyDirectionName,
        CourseNumber: studyPlan?.CourseNumber
    );

    public static void ApplyUpdate(Student student, UpdateStudentDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            student.FirstName = dto.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(dto.LastName))
            student.LastName = dto.LastName.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Patronymic))
            student.Patronymic = dto.Patronymic.Trim();

        if (dto.ContactPhone != null) student.ContactPhone = dto.ContactPhone;
        if (dto.ContactEmail != null) student.ContactEmail = dto.ContactEmail;
        if (dto.CityId.HasValue) student.CityId = dto.CityId.Value;
        if (dto.AvatarUrl != null) student.AvatarUrl = dto.AvatarUrl;

        if (dto.BirthDate.HasValue) student.BirthDate = dto.BirthDate;
        if (dto.IsForeign.HasValue) student.IsForeign = dto.IsForeign;

        if (dto.Gender != null && Enum.TryParse<Student.StudentGender>(dto.Gender, out var gender))
            student.Gender = gender;
        if (dto.Status != null && Enum.TryParse<Student.StudentStatus>(dto.Status, out var status))
            student.Status = status;
    }
}
