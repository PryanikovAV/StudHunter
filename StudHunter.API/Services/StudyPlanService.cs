using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.StudyPlan;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class StudyPlanService(StudHunterDbContext context) : BaseStudyPlanService(context)
{
    /// <summary>
    /// Creates a new study plan for a student.
    /// </summary>
    /// <param name="dto">The data transfer object containing study plan details.</param>
    /// <returns>A tuple containing the created study plan DTO, an optional status code, and an optional error message.</returns>
    public async Task<(StudyPlanDto? Entiry, int? StatusCode, string? ErrorMessage)> CreateStudyPlanAsync(CreateStudyPlanDto dto)
    {
        if (await _context.StudyPlans.AnyAsync(sp => sp.StudentId == dto.StudentId))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(StudyPlan), nameof(Student)));

        if (!await _context.Students.AnyAsync(s => s.Id == dto.StudentId && !s.IsDeleted))
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));

        if (!await _context.Faculties.AnyAsync(f => f.Id == dto.FacultyId))
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Faculty)));

        if (!await _context.Specialities.AnyAsync(s => s.Id == dto.SpecialityId))
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Speciality)));

        var invalidCourseIds = dto.CourseIds.Where(id => !_context.Courses.Any(c => c.Id == id)).ToList();
        if (invalidCourseIds.Any())
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Course)));

        var studyPlan = new StudyPlan
        {
            Id = Guid.NewGuid(),
            StudentId = dto.StudentId,
            CourseNumber = dto.CourseNumber,
            FacultyId = dto.FacultyId,
            SpecialityId = dto.SpecialityId,
            StudyForm = Enum.Parse<StudyPlan.StudyForms>(dto.StudyForm),
            BeginYear = dto.BeginYear,
            StudyPlanCourses = dto.CourseIds.Select(courseId => new StudyPlanCourse
            {
                StudyPlanId = Guid.NewGuid(),
                CourseId = courseId
            }).ToList()
        };

        _context.StudyPlans.Add(studyPlan);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<StudyPlan>();

        if (!success)
            return (null, statusCode, errorMessage);

        var createdStudyPlan = await _context.StudyPlans
        .Include(sp => sp.StudyPlanCourses)
        .FirstOrDefaultAsync(sp => sp.Id == studyPlan.Id);

        return (MapToStudyPlanDto(createdStudyPlan!), null, null);
    }

    /// <summary>
    /// Updates an existing study plan.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the study plan.</param>
    /// <param name="dto">The data transfer object containing updated study plan details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudyPlanAsync(Guid id, UpdateStudyPlanDto dto)
    {
        var studyPlan = await _context.StudyPlans
        .Include(sp => sp.StudyPlanCourses)
        .FirstOrDefaultAsync(sp => sp.Id == id);

        if (studyPlan == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(StudyPlan)));

        if (dto.FacultyId.HasValue && !await _context.Faculties.AnyAsync(f => f.Id == dto.FacultyId))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Faculty)));

        if (dto.SpecialityId.HasValue && !await _context.Specialities.AnyAsync(s => s.Id == dto.SpecialityId))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Speciality)));

        if (dto.CourseIds != null)
        {
            var invalidCourseIds = dto.CourseIds.Where(id => !_context.Courses.Any(c => c.Id == id)).ToList();
            if (invalidCourseIds.Any())
                return (false, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Course)));

            _context.StudyPlanCourses.RemoveRange(studyPlan.StudyPlanCourses);

            studyPlan.StudyPlanCourses = dto.CourseIds.Select(courseId => new StudyPlanCourse
            {
                StudyPlanId = studyPlan.Id,
                CourseId = courseId
            }).ToList();
        }

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

        return await SaveChangesAsync<StudyPlan>();
    }
}
