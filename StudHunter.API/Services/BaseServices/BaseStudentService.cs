using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseStudentService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Maps a Student entity to a StudentDto.
    /// </summary>
    /// <param name="student">The student entity to map.</param>
    /// <returns>A TDto representing the student.</returns>
    protected TDto MapToStudentDto<TDto>(Student student) where TDto : StudentDto, new()
    {
        var dto = new TDto
        {
            Id = student.Id,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            ContactPhone = student.ContactPhone,
            ContactEmail = student.ContactEmail,
            CreatedAt = student.CreatedAt,
            Gender = student.Gender.ToString(),
            BirthDate = student.BirthDate == DateOnly.MinValue ? null : student.BirthDate,
            Photo = student.Photo,
            IsForeign = student.IsForeign,
            Status = student.Status.ToString(),
            ResumeId = student.Resume?.Id,
            Achievements = student.Achievements.Select(BaseUserAchievementService.MapToUserAchievementDto).ToList(),
        };

        if (dto is AdminStudentDto adminDto)
        {
            adminDto.IsDeleted = student.IsDeleted;
        }

        return dto;
    }

    /// <summary>
    /// Retrieves an student by their ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple containing the student's details, an optional status code, and an optional error message.</returns>
    public async Task<(TDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync<TDto>(Guid id) where TDto : StudentDto, new()
    {
        var student = await _context.Students
        .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
        .Include(s => s.Resume)
        .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (student == null)
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));

        return (MapToStudentDto<TDto>(student), null, null);
    }

    /// <summary>
    /// Retrieves a student by their email.
    /// </summary>
    /// <param name="email">The email of the student.</param>
    /// <returns>A typle containing the student DTO, an optional status code, and an optional error message.</returns>
    public async Task<(TDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentByEmailAsync<TDto>(string email) where TDto : StudentDto, new()
    {
        var student = await _context.Students
        .Include(s => s.Achievements).ThenInclude(a => a.AchievementTemplate)
        .Include(s => s.Resume)
        .FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);

        if (student == null)
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));

        return (MapToStudentDto<TDto>(student), null, null);
    }
}
