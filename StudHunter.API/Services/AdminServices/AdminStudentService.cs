using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Student;
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
        .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
        .Include(s => s.Resume)
        .ToListAsync();

        var dtos = students.Select(MapToStudentDto<AdminStudentDto>).ToList();

        return (dtos, null, null);
    }

    /// <summary>
    /// Updates a student's profile by administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudentAsync(Guid id, AdminUpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));

        if (dto.Email != null && await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != id && !s.IsDeleted))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), "email"));

        if (dto.ContactPhone != null && await _context.Students.AnyAsync(s => s.ContactPhone == dto.ContactPhone && s.Id != id && !s.IsDeleted))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), "ContactPhone"));

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
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudentAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Student>(id, hardDelete);
    }
}
