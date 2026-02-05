using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseResumeService(StudHunterDbContext context) : BaseService(context)
{
    protected IQueryable<Resume> GetFullResumeQuery() =>
        _context.Resumes
            .Include(r => r.Student).ThenInclude(s => s.StudyPlan!).ThenInclude(sp => sp.Faculty)
            .Include(r => r.Student).ThenInclude(s => s.StudyPlan!).ThenInclude(sp => sp.Speciality)
            .Include(r => r.AdditionalSkills).ThenInclude(ras => ras.AdditionalSkill);

    public async Task<Result<ResumeDto>> GetResumeByStudentIdAsync(Guid studentId, Guid? currentUserId = null, bool ignoreFilters = true)
    {
        var query = GetFullResumeQuery();
        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        var resume = await query.FirstOrDefaultAsync(r => r.StudentId == studentId);
        if (resume == null)
            return Result<ResumeDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        if (currentUserId.HasValue)
        {
            var blackListCheck = await EnsureCommunicationAllowedAsync(currentUserId.Value, resume.StudentId);
            if (!blackListCheck.IsSuccess)
                return Result<ResumeDto>.Failure(ErrorMessages.CommunicationBlocked(), StatusCodes.Status403Forbidden);
        }

        return Result<ResumeDto>.Success(ResumeMapper.ToDto(resume));
    }

    protected void UpdateResumeSkills(Resume resume, List<Guid>? newSkillIds)
    {
        var safeIds = newSkillIds ?? [];
        var currentIds = resume.AdditionalSkills.Select(s => s.AdditionalSkillId).ToList();

        var toRemove = resume.AdditionalSkills.Where(s => !safeIds.Contains(s.AdditionalSkillId)).ToList();
        foreach (var item in toRemove) resume.AdditionalSkills.Remove(item);

        var toAdd = safeIds.Except(currentIds)
            .Select(id => new ResumeAdditionalSkill { AdditionalSkillId = id, ResumeId = resume.Id });
        foreach (var item in toAdd) resume.AdditionalSkills.Add(item);
    }

    public async Task<Result<bool>> SoftDeleteResumeAsync(Guid studentId)
    {
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.StudentId == studentId);
        if (resume == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        resume.IsDeleted = true;
        resume.DeletedAt = DateTime.UtcNow;

        return (await SaveChangesAsync<Resume>()).IsSuccess
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(ErrorMessages.FailedToDelete(nameof(Resume)));
    }

    public async Task<Result<ResumeDto>> RestoreResumeAsync(Guid studentId)
    {
        var resume = await _context.Resumes.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.StudentId == studentId);
        if (resume == null)
            return Result<ResumeDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        resume.IsDeleted = false;
        resume.DeletedAt = null;

        await _context.SaveChangesAsync();
        return await GetResumeByStudentIdAsync(studentId, null, true);
    }
}
