using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing students.
/// </summary>
public class StudentService(StudHunterDbContext context, UserAchievementService userAchievementService, IPasswordHasher passwordHasher)
    : BaseStudentService(context, userAchievementService)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    /// <summary>
    /// Creates a new student and their study plan.
    /// </summary>
    /// <param name="dto">The data transfer object containing student details.</param>
    /// <returns>A tuple containing the created student, an optional status code, and an optional error message.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> CreateStudentAsync(CreateStudentDto dto)
    {
        #region Serializers
        if (await _context.Students.AnyAsync(s => s.Email == dto.Email && !s.IsDeleted))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Student), "email"));

        if (!await _context.Faculties.AnyAsync(f => f.Id == dto.FacultyId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Faculty)));

        if (!await _context.Specialities.AnyAsync(s => s.Id == dto.SpecialityId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Speciality)));
        #endregion

        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Gender = Enum.Parse<Student.StudentGender>(dto.Gender),
            BirthDate = dto.BirthDate,
            Photo = dto.Photo,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            IsForeign = dto.IsForeign,
            StatusId = dto.StatusId,
            CreatedAt = DateTime.UtcNow
        };

        var studyPlan = new StudyPlan
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            Student = student,
            CourseNumber = dto.CourseNumber,
            FacultyId = dto.FacultyId,
            SpecialityId = dto.SpecialityId,
            StudyForm = Enum.Parse<StudyPlan.StudyForms>(dto.StudyForm),
            BeginYear = dto.BeginYear
        };

        _context.Students.Add(student);
        _context.StudyPlans.Add(studyPlan);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Student>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Gender = student.Gender.ToString(),
            BirthDate = student.BirthDate,
            Photo = student.Photo,
            ContactPhone = student.ContactPhone,
            ContactEmail = student.ContactEmail,
            IsForeign = student.IsForeign,
            StatusId = student.StatusId,
            ResumeId = null,
            CreatedAt = student.CreatedAt,
            CourseNumber = studyPlan.CourseNumber,
            FacultyId = studyPlan.FacultyId,
            SpecialityId = studyPlan.SpecialityId,
            StudyForm = studyPlan.StudyForm.ToString(),
            BeginYear = studyPlan.BeginYear
        }, null, null);
    }

    /// <summary>
    /// Updates an existing student and their study plan.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudentAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _context.Students
        .Include(s => s.StudyPlan)
        .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        #region Serializers
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Student)));

        if (dto.Email != null)
        {
            if (await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != id && !s.IsDeleted))
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Student), "email"));
        }

        if (dto.FacultyId.HasValue)
        {
            if (!await _context.Faculties.AnyAsync(f => f.Id == dto.FacultyId.Value))
                return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Faculty)));
        }

        if (dto.SpecialityId.HasValue)
        {
            if (!await _context.Specialities.AnyAsync(s => s.Id == dto.SpecialityId.Value))
                return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Speciality)));
        }
        #endregion

        if (dto.FirstName != null)
            student.FirstName = dto.FirstName;
        if (dto.LastName != null)
            student.LastName = dto.LastName;
        if (dto.Email != null)
            student.Email = dto.Email;
        if (dto.Password != null)
            student.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        if (dto.Gender != null)
            student.Gender = Enum.Parse<Student.StudentGender>(dto.Gender);
        if (dto.BirthDate.HasValue)
            student.BirthDate = dto.BirthDate.Value;
        if (dto.Photo != null)
            student.Photo = dto.Photo;
        if (dto.ContactPhone != null)
            student.ContactPhone = dto.ContactPhone;
        if (dto.ContactEmail != null)
            student.ContactEmail = dto.ContactEmail;
        if (dto.IsForeign.HasValue)
            student.IsForeign = dto.IsForeign.Value;
        if (dto.StatusId.HasValue)
            student.StatusId = dto.StatusId.Value;

        if (dto.CourseNumber.HasValue || dto.FacultyId.HasValue || dto.SpecialityId.HasValue || dto.StudyForm != null || dto.BeginYear.HasValue)
        {
            var studyPlan = student.StudyPlan;
            if (dto.CourseNumber.HasValue)
                studyPlan.CourseNumber = dto.CourseNumber.Value;
            if (dto.FacultyId.HasValue)
                studyPlan.FacultyId = dto.FacultyId.Value;
            if (dto.SpecialityId.HasValue)
                studyPlan.SpecialityId = dto.SpecialityId.Value;
            if (dto.StudyForm != null)
                studyPlan.StudyForm = Enum.Parse<StudyPlan.StudyForms>(dto.StudyForm);
            if (dto.BeginYear.HasValue)
                studyPlan.BeginYear = dto.BeginYear.Value;
        }

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Student>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a student (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudentAsync(Guid id)
    {
        return await DeleteEntityAsync<Student>(id);
    }
}
