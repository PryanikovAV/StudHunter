using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.StudyPlanDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class StudyPlanService(StudHunterDbContext context) : BaseStudyPlanService(context)
{
    /// <summary>
    /// Retrieves a study plan by studentId.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="authUserId"></param>
    /// <returns>A tuple containing the study plan DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudyPlanDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudyPlanAsync(Guid authUserId, Guid studentId)
    {
        var student = await _context.Students
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Id != authUserId)
            return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", nameof(StudyPlan)));
        if (student.StudyPlan == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(StudyPlan)));

        return (MapToStudyPlanDto(student.StudyPlan), null, null);
    }

    /// <summary>
    /// Creates a new study plan for a student.
    /// </summary>
    /// <param name="authUserId"></param>
    /// <param name="dto">The data transfer object containing study plan details.</param>
    /// <returns>A tuple containing the created study plan DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudyPlanDto? Entiry, int? StatusCode, string? ErrorMessage)> CreateStudyPlanAsync(Guid authUserId, CreateStudyPlanDto dto)
    {
        if (authUserId != dto.StudentId)
            return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("create", nameof(StudyPlan)));
        if (!await _context.Students.AnyAsync(s => s.Id == dto.StudentId))
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));
        if (await _context.StudyPlans.IgnoreQueryFilters().AnyAsync(sp => sp.StudentId == dto.StudentId))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(StudyPlan), nameof(Student)));

        var studyPlan = new StudyPlan
        {
            Id = Guid.NewGuid(),
            StudentId = dto.StudentId,
            CourseNumber = dto.CourseNumber,
            FacultyId = dto.FacultyId,
            SpecialityId = dto.SpecialityId,
            StudyForm = dto.StudyForm,
            StudyPlanCourses = dto.CourseIds.Select(courseId => new StudyPlanCourse
            {
                StudyPlanId = Guid.NewGuid(),
                CourseId = courseId
            }).ToList(),
            IsDeleted = false
        };

        _context.StudyPlans.Add(studyPlan);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<StudyPlan>();
        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToStudyPlanDto(studyPlan), null, null);
    }

    /// <summary>
    /// Updates an existing study plan.
    /// </summary>
    /// <param name="authUserId"></param>
    /// <param name="studentId">The unique identifier (GUID) of the study plan.</param>
    /// <param name="dto">The data transfer object containing updated study plan details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudyPlanAsync(Guid authUserId, Guid studentId, UpdateStudyPlanDto dto)
    {
        if (studentId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", nameof(StudyPlan)));
        var student = await _context.Students
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.StudyPlan == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(StudyPlan)));
        if (await _context.StudyPlans.IgnoreAutoIncludes().AnyAsync(sp => sp.StudentId == studentId))
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.EntityAlreadyDeleted(nameof(StudyPlan), "RecoveryAccount"));

        if (dto.CourseNumber.HasValue)
            student.StudyPlan.CourseNumber = dto.CourseNumber.Value;
        if (dto.FacultyId.HasValue)
            student.StudyPlan.FacultyId = dto.FacultyId.Value;
        if (dto.SpecialityId.HasValue)
            student.StudyPlan.SpecialityId = dto.SpecialityId.Value;
        if (dto.StudyForm.HasValue)
            student.StudyPlan.StudyForm = dto.StudyForm.Value;
        if (dto.CourseIds != null)
        {
            _context.StudyPlanCourses.RemoveRange(student.StudyPlan.StudyPlanCourses);
            student.StudyPlan.StudyPlanCourses = dto.CourseIds.Select(courseId => new StudyPlanCourse
            {
                StudyPlanId = student.StudyPlan.Id,
                CourseId = courseId
            }).ToList();
        }

        return await SaveChangesAsync<StudyPlan>();
    }
}
