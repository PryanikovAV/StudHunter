using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseResumeService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected IQueryable<Resume> GetFullResumeQuery(bool ignoreFilters = false)
    {
        var query = _context.Resumes.AsQueryable();

        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        return query
            .Include(r => r.Student)
                .ThenInclude(s => s!.StudyPlan).ThenInclude(sp => sp!.Faculty)
            .Include(r => r.Student)
                .ThenInclude(s => s!.StudyPlan).ThenInclude(sp => sp!.StudyDirection)
            .Include(r => r.AdditionalSkills)
                .ThenInclude(ras => ras.AdditionalSkill);
    }

    protected async Task<Result<bool>> SoftDeleteResumeInternalAsync(Guid studentId)
    {
        var student = await _context.Students
            .Include(s => s.Resume)
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student?.Resume == null || student.Resume.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        student.Resume.IsDeleted = true;
        student.Resume.DeletedAt = DateTime.UtcNow;

        _registrationManager.RecalculateRegistrationStage(student);

        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    protected async Task<Result<bool>> RestoreResumeInternalAsync(Guid studentId)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.Resume)
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student?.Resume == null || !student.Resume.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        student.Resume.IsDeleted = false;
        student.Resume.DeletedAt = null;

        _registrationManager.RecalculateRegistrationStage(student);

        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}