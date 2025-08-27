using StudHunter.API.ModelsDto.Student;
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
            Achievements = student.Achievements
                .Select(BaseUserAchievementService.MapToUserAchievementDto)
                .ToList(),
        };

        if (dto is AdminStudentDto adminDto)
        {
            adminDto.IsDeleted = student.IsDeleted;
        }

        return dto;
    }
}
