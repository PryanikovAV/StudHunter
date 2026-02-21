using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseStudentService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected IQueryable<Student> GetStudentQuery(
        bool asNoTracking = false,
        bool ignoreFilters = false,
        bool includeCourses = false,
        bool includeStudyPlanDictionaries = false)
    {
        var query = _context.Students.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();

        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        query = query
            .Include(s => s.Resume)
            .Include(s => s.StudyPlan);

        if (includeStudyPlanDictionaries)
        {
            query = query
                .Include(s => s.StudyPlan!)
                    .ThenInclude(sp => sp.University)
                .Include(s => s.StudyPlan!)
                    .ThenInclude(sp => sp.Faculty)
                .Include(s => s.StudyPlan!)
                    .ThenInclude(sp => sp.Department)
                .Include(s => s.StudyPlan!)
                    .ThenInclude(sp => sp.StudyDirection);
        }

        if (includeCourses)
        {
            query = query
                .Include(s => s.StudyPlan!)
                    .ThenInclude(sp => sp.StudyPlanCourses)
                        .ThenInclude(spc => spc.Course);
        }

        return query;
    }

    protected async Task SoftDeleteStudentAsync(Student student, DateTime now)
    {
        student.IsDeleted = true;
        student.DeletedAt = now;

        if (student.Resume != null && !student.Resume.IsDeleted)
        {
            student.Resume.IsDeleted = true;
            student.Resume.DeletedAt = now;
        }

        if (student.StudyPlan != null && !student.StudyPlan.IsDeleted)
        {
            student.StudyPlan.IsDeleted = true;
            student.StudyPlan.DeletedAt = now;
        }

        await ClearUserActivityAsync(student.Id, now);

        _registrationManager.RecalculateRegistrationStage(student);
    }
}
