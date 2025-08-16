using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using StudHunter.API.ModelsDto.StudyPlan;
using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseStudyPlanService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Maps a StudyPlan entity to a StudyPlanDto.
    /// </summary>
    /// <param name="studyPlan">The study plan entity to map.</param>
    /// <returns>A StudyPlanDto representing the study plan.</returns>
    protected StudyPlanDto MapToStudyPlanDto(StudyPlan studyPlan)
    {
        return new StudyPlanDto
        {
            Id = studyPlan.Id,
            StudentId = studyPlan.StudentId,
            CourseNumber = studyPlan.CourseNumber,
            FacultyId = studyPlan.FacultyId,
            SpecialityId = studyPlan.SpecialityId,
            StudyForm = studyPlan.StudyForm.ToString(),
            BeginYear = studyPlan.BeginYear,
            CourseIds = studyPlan.StudyPlanCourses.Select(spc => spc.CourseId).ToList()
        };
    }

    /// <summary>
    /// Retrieves a study plan by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the study plan.</param>
    /// <returns>A tuple containing the study plan DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudyPlanDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudyPlanAsync(Guid id)
    {
        var studyPlan = await _context.StudyPlans
        .Include(sp => sp.StudyPlanCourses)
        .FirstOrDefaultAsync(sp => sp.Id == id);

        if (studyPlan == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(StudyPlan)));

        return (MapToStudyPlanDto(studyPlan), null, null);
    }

    /// <summary>
    /// Retrieves a study plan by student ID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple containing the study plan DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudyPlanDto? Entity, int? StatusCode, string? ErrorrMessage)> GetStudyPlanByStudentAsync(Guid studentId)
    {
        var studyPlan = await _context.StudyPlans
        .Include(sp => sp.StudyPlanCourses)
        .FirstOrDefaultAsync(sp => sp.StudentId == studentId);

        if (studyPlan == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(StudyPlan)));

        return (MapToStudyPlanDto(studyPlan), null, null);
    }
}
