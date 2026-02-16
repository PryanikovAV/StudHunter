using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseStudentService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected async Task<Student?> GetStudentInternalAsync(Guid studentId, bool ignoreFilters = false)
    {
        var query = _context.Students.AsQueryable();

        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        return await query
                .Include(s => s.Resume)
                .Include(s => s.StudyPlan)
                .FirstOrDefaultAsync(s => s.Id == studentId);
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
