using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<Resume> IncludeFullResumeDetails(this IQueryable<Resume> query)
    {
        return query
            .Include(r => r.Student).ThenInclude(s => s.City)
            .Include(r => r.Student).ThenInclude(s => s.StudyPlan).ThenInclude(sp => sp!.University)
            .Include(r => r.Student).ThenInclude(s => s.StudyPlan).ThenInclude(sp => sp!.Faculty)
            .Include(r => r.Student).ThenInclude(s => s.StudyPlan).ThenInclude(sp => sp!.Department)
            .Include(r => r.Student).ThenInclude(s => s.StudyPlan).ThenInclude(sp => sp!.StudyDirection)
            .Include(r => r.Student).ThenInclude(s => s.StudyPlan).ThenInclude(sp => sp!.StudyPlanCourses).ThenInclude(spc => spc.Course)
            .Include(r => r.AdditionalSkills).ThenInclude(s => s.AdditionalSkill);
    }
}