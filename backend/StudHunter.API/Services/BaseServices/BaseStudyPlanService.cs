using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseStudyPlanService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected IQueryable<StudyPlan> GetFullStudyPlanQuery() =>
        _context.StudyPlans
            .IgnoreQueryFilters()
            .Include(sp => sp.University)
            .Include(sp => sp.Faculty)
            .Include(sp => sp.Department)
            .Include(sp => sp.StudyDirection)
            .Include(sp => sp.StudyPlanCourses)
                .ThenInclude(spc => spc.Course);
}
