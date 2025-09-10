using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.StudyPlanDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminStudyPlanService(StudHunterDbContext context) : BaseStudyPlanService(context)
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<(List<StudyPlanDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllStudyPlansAsync()
    {
        var studyPlans = await _context.StudyPlans
            .IgnoreQueryFilters()
            .Include(sp => sp.StudyPlanCourses)
            .ToListAsync();

        return (studyPlans.Select(MapToStudyPlanDto).ToList(), null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="courseNumber"></param>
    /// <param name="facultyId"></param>
    /// <param name="specialityId"></param>
    /// <param name="courseIds"></param>
    /// <returns></returns>
    public async Task<(List<StudyPlanDto>? Entities, int? StatusCode, string? ErrorMessage)> SearchStudyPlansAsync(
    int? courseNumber, Guid? facultyId, Guid? specialityId, List<Guid>? courseIds)
    {
        var query = _context.StudyPlans
            .IgnoreQueryFilters()
            .Include(sp => sp.Student)
            .Include(sp => sp.StudyPlanCourses)
            .Where(sp => !sp.Student.IsDeleted);

        if (courseNumber.HasValue)
            query = query.Where(sp => sp.CourseNumber == courseNumber.Value);
        if (facultyId.HasValue)
            query = query.Where(sp => sp.FacultyId == facultyId.Value);
        if (specialityId.HasValue)
            query = query.Where(sp => sp.SpecialityId == specialityId.Value);
        if (courseIds != null && courseIds.Any())
            query = query.Where(sp => sp.StudyPlanCourses.Any(spc => courseIds.Contains(spc.CourseId)));

        var studyPlans = await query
            .Select(sp => MapToStudyPlanDto(sp))
            .ToListAsync();

        return (studyPlans, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudyPlanAsync(Guid studentId, UpdateStudyPlanDto dto)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.StudyPlan == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(StudyPlan)));

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

    /// <summary>
    /// Deletes a study plan.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudyPlanAsync(Guid studentId, bool hardDelete = false)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.StudyPlan)
            .Include(s => s.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.StudyPlan == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(StudyPlan)));
        if (student.Resume == null)
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(Resume)));

        if (hardDelete)
        {
            _context.StudyPlans.Remove(student.StudyPlan);
        }
        else
        {
            student.StudyPlan.IsDeleted = true;
            student.StudyPlan.DeletedAt = DateTime.UtcNow;
            student.Resume.IsDeleted = true;
            student.Resume.DeletedAt = DateTime.UtcNow;
        }

        return await SaveChangesAsync<StudyPlan>();
    }
}
