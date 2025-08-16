using StudHunter.API.Services.BaseServices;
using StudHunter.API.ModelsDto.StudyPlan;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace StudHunter.API.Services.AdminServices;

public class AdminStudyPlanService(StudHunterDbContext context) : BaseStudyPlanService(context)
{
    /// <summary>
    /// Retrieves all study plans.
    /// </summary>
    /// <returns>A tuple containing a list of all study plans, an optional status code, and an optional error message.</returns>
    public async Task<(List<StudyPlanDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllStudyPlansAsync()
    {
        var studyPlans = await _context.StudyPlans
        .Include(sp => sp.StudyPlanCourses)
        .ToListAsync();

        var dtos = studyPlans.Select(MapToStudyPlanDto).ToList();

        return (dtos, null, null);
    }

    /// <summary>
    /// Deletes a study plan.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the study plan.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudyPlanAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<StudyPlan>(id, hardDelete);
    }
}
