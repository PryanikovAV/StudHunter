using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.StudentDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing students with administrative privileges.
/// </summary>
public class AdminStudentService(StudHunterDbContext context) : BaseStudentService(context)
{
    /// <summary>
    /// Retrieves all students include deleted.
    /// </summary>
    /// <returns>A tuple containing a list of all students, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminStudentDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllStudentsAsync()
    {
        var students = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
            .Include(s => s.Resume)
            .Select(s => MapToStudentDto<AdminStudentDto>(s))
            .OrderByDescending(s => s.LastName)
            .ToListAsync();

        return (students, null, null);
    }

    /// <summary>
    /// Retrieves an student by their ID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple containing the student's details, an optional status code, and an optional error message.</returns>
    public async Task<(AdminStudentDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync(Guid studentId)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
       
        return (MapToStudentDto<AdminStudentDto>(student), null, null);
    }

    /// <summary>
    /// Retrieves a student by their email.
    /// </summary>
    /// <param name="email">The email of the student.</param>
    /// <returns>A typle containing the student DTO, an optional status code, and an optional error message.</returns>
    public async Task<(AdminStudentDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync(string email)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Email == email);
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        
        return (MapToStudentDto<AdminStudentDto>(student), null, null);
    }

    /// <summary>
    /// Updates a student's profile by administrator.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudentAsync(Guid studentId, AdminUpdateStudentDto dto)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (dto.Email != null && await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != studentId && !s.IsDeleted))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), nameof(Student.Email)));
        if (dto.ContactPhone != null && await _context.Students.AnyAsync(s => s.ContactPhone == dto.ContactPhone && s.Id != studentId && !s.IsDeleted))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), nameof(Student.ContactPhone)));

        if (dto.Email != null)
            student.UpdateEmail(dto.Email);
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
        if (dto.IsDeleted.HasValue)
            student.IsDeleted = dto.IsDeleted.Value;

        return await SaveChangesAsync<Student>();
    }

    /// <summary>
    /// Deletes a student (hard or soft delete).
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudentAsync(Guid studentId, bool hardDelete = false)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.StudyPlan)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));

        if (hardDelete)
        {
            _context.Students.Remove(student);
        }
        else
        {
            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;

            if (student.StudyPlan != null && !student.StudyPlan.IsDeleted)
            {
                student.StudyPlan.IsDeleted = true;
                student.StudyPlan.DeletedAt = DateTime.UtcNow;
            }

            if (student.Resume != null && !student.Resume.IsDeleted)
            {
                student.Resume.IsDeleted = true;
                student.Resume.DeletedAt = DateTime.UtcNow;
            }

            var invitations = await _context.Invitations
                .Where(i => (i.SenderId == student.Id || i.ReceiverId == student.Id) && i.Status != Invitation.InvitationStatus.Rejected)
                .ToListAsync();

            foreach (var invitation in invitations)
            {
                invitation.Status = Invitation.InvitationStatus.Rejected;
                invitation.UpdatedAt = DateTime.UtcNow;
            }

            var favorites = await _context.Favorites
                .Where(f => f.StudentId == student.Id)
                .ToListAsync();

            _context.Favorites.RemoveRange(favorites);
        }

        return await SaveChangesAsync<Student>();
    }
}
