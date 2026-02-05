using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseStudentService(StudHunterDbContext context) : BaseService(context)
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

        RecalculateRegistrationStage(student);
    }

    protected void RecalculateRegistrationStage(Student student)
    {
        if (student.IsDeleted)
        {
            student.RegistrationStage = User.AccountStatus.Anonymous;
            return;
        }

        bool isProfileComplete =
            student.FirstName != UserDefaultNames.DefaultFirstName &&
            student.LastName != UserDefaultNames.DefaultLastName &&
            student.Gender.HasValue &&
            student.BirthDate.HasValue &&
            student.StudyPlan != null &&
            student.StudyPlan.FacultyId != null;

        bool hasActiveResume = student.Resume != null && !student.Resume.IsDeleted;

        if (hasActiveResume && isProfileComplete)
            student.RegistrationStage = User.AccountStatus.FullyActivated;
        else if (isProfileComplete)
            student.RegistrationStage = User.AccountStatus.ProfileFilled;
        else
            student.RegistrationStage = User.AccountStatus.Anonymous;
    }
}
