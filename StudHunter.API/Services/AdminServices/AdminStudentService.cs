using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminStudentService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<(List<AdminStudentDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllStudentsAsync()
    {
        var students = await _context.Students
        .Include(s => s.Resume)
        .Include(s => s.StudyPlan)
        .Include(s => s.Achievements)
        .ThenInclude(ua => ua.AchievementTemplate)
        .Select(s => new AdminStudentDto
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            Gender = s.Gender.ToString(),
            BirthDate = s.BirthDate,
            Photo = s.Photo,
            ContactPhone = s.ContactPhone,
            ContactEmail = s.ContactEmail,
            IsForeign = s.IsForeign,
            StatusId = s.StatusId,
            ResumeId = s.Resume != null ? s.Resume.Id : null,
            CreatedAt = s.CreatedAt,
            IsDeleted = s.IsDeleted,
            CourseNumber = s.StudyPlan.CourseNumber,
            FacultyId = s.StudyPlan.FacultyId,
            SpecialityId = s.StudyPlan.SpecialityId,
            StudyForm = s.StudyPlan.StudyForm.ToString(),
            BeginYear = s.StudyPlan.BeginYear,
            Achievements = s.Achievements.Select(userAchievement => new UserAchievementDto
            {
                Id = userAchievement.Id,
                UserId = userAchievement.UserId,
                AchievementTemplateOrderNumber = userAchievement.AchievementTemplate.OrderNumber,
                AchievementAt = userAchievement.AchievementAt,
                AchievementName = userAchievement.AchievementTemplate.Name,
                AchievementDescription = userAchievement.AchievementTemplate.Description
            }).ToList()
        }).ToListAsync();

        return (students, null, null);
    }

    public async Task<(AdminStudentDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync(Guid id)
    {
        var student = await _context.Students
        .Include(s => s.Resume)
        .Include(s => s.StudyPlan)
        .Include(s => s.Achievements)
        .ThenInclude(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(s => s.Id == id);

        #region Serializers
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Student"));
        #endregion

        return (new AdminStudentDto
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
            IsDeleted = student.IsDeleted,
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

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudentAsync(Guid id, AdminUpdateStudentDto dto)
    {
        var student = await _context.Students
        .Include(s => s.StudyPlan)
        .FirstOrDefaultAsync(s => s.Id == id);

        #region Serializers
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Student"));

        if (dto.Email != null)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != id);
            if (studentExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Student", "Email"));
        }

        if (dto.FacultyId.HasValue)
        {
            var facultyExists = await _context.Faculties.AnyAsync(f => f.Id == dto.FacultyId.Value);
            if (facultyExists == false)
                return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Faculty"));
        }

        if (dto.SpecialityId.HasValue)
        {
            var specialityExists = await _context.Specialities.AnyAsync(s => s.Id == dto.SpecialityId.Value);
            if (specialityExists)
                return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Speciality"));
        }
        #endregion

        if (dto.FirstName != null)
            student.FirstName = dto.FirstName;
        if (dto.LastName != null)
            student.LastName = dto.LastName;
        if (dto.Email != null)
            student.Email = dto.Email;
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
        if (dto.IsDeleted.HasValue)
            student.IsDeleted = dto.IsDeleted.Value;

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

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudentAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Student>(id, hardDelete);
    }
}
