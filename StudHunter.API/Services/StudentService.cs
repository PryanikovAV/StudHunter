using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;
using StudHunter.API.ModelsDto.Auth;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing students.
/// </summary>
public class StudentService(StudHunterDbContext context, IPasswordHasher passwordHasher, AuthService authService) : BaseStudentService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Registers a new student with minimal details (email, password, first name, last name, gender, status, isForeign).
    /// </summary>
    /// <param name="dto">The data transfer object containing student registration details.</param>
    /// <returns>A tuple containing the student DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> CreateStudentAsync(RegisterStudentDto dto)
    {
        if (!new EmailAddressAttribute().IsValid(dto.Email))
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.InvalidData("email format"));

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), "email"));

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

        _context.Students.Add(student);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Student>();

        if (!success)
            return (null, statusCode, errorMessage);

        var studentEntity = await _context.Students
        .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
        .Include(s => s.Resume)
        .FirstOrDefaultAsync(s => s.Id == student.Id && !s.IsDeleted);

        var token = _authService.GenerateJwtToken(student.Id, nameof(Student));

        return (MapToStudentDto<StudentDto>(studentEntity!), null, null);
    }

    /// <summary>
    /// Updates a student's profile.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated student details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudentAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _context.Students
        .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));

        if (dto.Email != null && await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != id && !s.IsDeleted))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), "email"));

        if (dto.ContactPhone != null && await _context.Students.AnyAsync(s => s.ContactPhone == dto.ContactPhone && s.Id != id && !s.IsDeleted))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), "ContactPhone"));

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
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudentAsync(Guid id)
    {
        return await DeleteEntityAsync<Student>(id);
    }
}
