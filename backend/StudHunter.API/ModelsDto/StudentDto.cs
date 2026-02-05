using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record StudentDto(
    Guid Id,
    string Email,
    string RegistrationStage,
    string FirstName,
    string LastName,
    string? ContactPhone,
    string? ContactEmail,
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
    string? ContactPhone,
    string? ContactEmail,
    string? Gender,
    DateOnly? BirthDate,
    string? AvatarUrl,
    bool? IsForeign,
    string Status,
    DateTime CreatedAt,
    Guid? ResumeId,
    bool IsDeleted)
    : StudentDto(Id, Email, RegistrationStage, FirstName, LastName, ContactPhone, ContactEmail,
                 Gender, BirthDate, AvatarUrl, IsForeign, Status, CreatedAt, ResumeId);

public record UpdateStudentDto(
    [StringLength(50)] string? FirstName,
    [StringLength(50)] string? LastName,
    [Phone] string? ContactPhone,
    [StringLength(255)] string? AvatarUrl,
    [EmailAddress] string? ContactEmail,
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
        s.ContactPhone,
        s.ContactEmail,
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
        s.ContactPhone,
        s.ContactEmail,
        s.Gender?.ToString(),
        s.BirthDate,
        s.AvatarUrl,
        s.IsForeign,
        s.Status.ToString(),
        s.CreatedAt,
        s.Resume?.Id,
        s.IsDeleted);

    public static void ApplyUpdate(Student student, UpdateStudentDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            student.FirstName = dto.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(dto.LastName))
            student.LastName = dto.LastName.Trim();

        if (dto.ContactPhone != null) student.ContactPhone = dto.ContactPhone;
        if (dto.ContactEmail != null) student.ContactEmail = dto.ContactEmail;
        if (dto.AvatarUrl != null) student.AvatarUrl = dto.AvatarUrl;

        if (dto.BirthDate.HasValue) student.BirthDate = dto.BirthDate;
        if (dto.IsForeign.HasValue) student.IsForeign = dto.IsForeign;

        if (dto.Gender != null && Enum.TryParse<Student.StudentGender>(dto.Gender, out var gender))
            student.Gender = gender;
        if (dto.Status != null && Enum.TryParse<Student.StudentStatus>(dto.Status, out var status))
            student.Status = status;
    }
}
