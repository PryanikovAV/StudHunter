using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.StudentDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using StudHunter.API.ModelsDto.AuthDto;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing students.
/// </summary>
public class StudentService(StudHunterDbContext context, IPasswordHasher passwordHasher, AuthService authService) : BaseStudentService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Retrieves an student by their ID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="authUserId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing the student's details, an optional status code, and an optional error message.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync(Guid authUserId, Guid studentId)
    {
        var student = await _context.Students
            .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Id != authUserId && await _context.Students.AnyAsync(s => s.Id == authUserId))
            return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", nameof(Student)));

        return (MapToStudentDto<StudentDto>(student), null, null);
    }

    /// <summary>
    /// Retrieves a student by their email.
    /// </summary>
    /// <param name="email">The email of the student.</param>
    /// <param name="authUserId">The unique identifier (GUID) of the user.</param>
    /// <returns>A typle containing the student DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync(Guid authUserId, string email)
    {
        var student = await _context.Students
            .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Email == email);
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Id != authUserId && await _context.Students.AnyAsync(s => s.Id == authUserId))
            return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", nameof(Student)));

        return (MapToStudentDto<StudentDto>(student), null, null);
    }

    /// <summary>
    /// Registers a new student with minimal details (email, password, first name, last name, gender, status, isForeign).
    /// </summary>
    /// <param name="dto">The data transfer object containing student registration details.</param>
    /// <returns>A tuple containing the student DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> CreateStudentAsync(RegisterStudentDto dto)
    {
        if (await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == dto.Email))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(User), nameof(User.Email)));

        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ContactPhone = dto.ContactPhone,
            BirthDate = dto.BirthDate ?? DateOnly.MinValue,
            Photo = dto.Photo,
            Gender = Enum.Parse<Student.StudentGender>(dto.Gender),
            Status = Enum.Parse<Student.StudentStatus>(dto.Status),
            IsForeign = dto.IsForeign,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        student.UpdateEmail(dto.Email);
        student.UpdatePassword(_passwordHasher.HashPassword(dto.Password));

        var studyPlan = new StudyPlan
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            CourseNumber = 1,
            FacultyId = Guid.Empty,
            SpecialityId = Guid.Empty,
            StudyForm = StudyPlan.StudyPlanForm.FullTime,
            IsDeleted = false
        };

        _context.Students.Add(student);
        _context.StudyPlans.Add(studyPlan);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Student>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToStudentDto<StudentDto>(student), null, null);
    }

    /// <summary>
    /// Updates a student's profile.
    /// </summary>
    /// <param name="authUserId"></param>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudentAsync(Guid authUserId, Guid studentId, UpdateStudentDto dto)
    {
        var student = await _context.Students
            .Include(s => s.StudyPlan)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Id != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", nameof(Student)));
        if (dto.Email != null && await _context.Students.IgnoreQueryFilters().AnyAsync(s => s.Email == dto.Email && s.Id != studentId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), nameof(Student.Email)));
        if (dto.ContactPhone != null && await _context.Students.AnyAsync(s => s.ContactPhone == dto.ContactPhone && s.Id != studentId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), nameof(Student.ContactPhone)));

        if (dto.Email != null)
            student.UpdateEmail(dto.Email);
        if (dto.Password != null)
            student.UpdatePassword(_passwordHasher.HashPassword(dto.Password));
        if (dto.FirstName != null)
            student.FirstName = dto.FirstName;
        if (dto.LastName != null)
            student.LastName = dto.LastName;
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
        if (dto.Status != null)
            student.Status = Enum.Parse<Student.StudentStatus>(dto.Status);

        return await SaveChangesAsync<Student>();
    }

    /// <summary>
    /// Deletes a student (soft delete).
    /// </summary>
    /// <param name="authUserId"></param>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudentAsync(Guid authUserId, Guid studentId)
    {
        var student = await _context.Students
            .Include(s => s.StudyPlan)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Id != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("delete", nameof(Student)));

        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;

        if (student.StudyPlan != null)
        {
            student.StudyPlan.IsDeleted = true;
            student.StudyPlan.DeletedAt = DateTime.UtcNow;
        }

        if (student.Resume != null)
        {
            student.Resume.IsDeleted = true;
            student.Resume.DeletedAt = DateTime.UtcNow;

            var invitations = await _context.Invitations
                .Where(i => i.ResumeId == student.Resume.Id && i.Status != Invitation.InvitationStatus.Rejected)
                .ToListAsync();

            foreach (var invitation in invitations)
            {
                invitation.Status = Invitation.InvitationStatus.Rejected;
                invitation.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await SaveChangesAsync<Student>();
    }
}
