using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public class BaseStudentService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

    /// <summary>
    /// Retrieves a student by their ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple containing the student, an optional status code, and an optional error message.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync(Guid id)
    {
        var student = await _context.Students
        .Include(s => s.Resume)
        .Include(s => s.StudyPlan)
        .Include(s => s.Achievements)
        .ThenInclude(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        #region Serializers
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Student)));

        if (student.StudyPlan == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(StudyPlan)));
        #endregion

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
            ResumeId = student.Resume != null ? student.Resume.Id : null,
            CreatedAt = student.CreatedAt,
            CourseNumber = student.StudyPlan.CourseNumber,
            FacultyId = student.StudyPlan.FacultyId,
            SpecialityId = student.StudyPlan.SpecialityId,
            StudyForm = student.StudyPlan.StudyForm.ToString(),
            BeginYear = student.StudyPlan.BeginYear,
            Achievements = student.Achievements.Select(userAchievement => new UserAchievementDto
            {
                Id = userAchievement.Id,
                UserId = userAchievement.UserId,
                AchievementTemplateOrderNumber = userAchievement.AchievementTemplate.OrderNumber,
                AchievementAt = userAchievement.AchievementAt,
                AchievementName = userAchievement.AchievementTemplate.Name,
                AchievementDescription = userAchievement.AchievementTemplate.Description
            }).ToList()
        }, null, null);
    }
}
