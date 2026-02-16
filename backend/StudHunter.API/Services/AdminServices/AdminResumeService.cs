using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminResumeService
{
    Task<Result<ResumeSearchDto>> GetResumeAsync(Guid studentId);
    Task<Result<ResumeFillDto>> UpdateResumeAsync(Guid studentId, ResumeFillDto dto);
    Task<Result<bool>> SoftDeleteResumeAsync(Guid studentId);
    Task<Result<bool>> HardDeleteResumeAsync(Guid studentId);
    Task<Result<bool>> RestoreResumeAsync(Guid studentId);
}

public class AdminResumeService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseResumeService(context, registrationManager), IAdminResumeService
{
    public async Task<Result<ResumeSearchDto>> GetResumeAsync(Guid studentId)
    {
        var resume = await GetFullResumeQuery(ignoreFilters: true)
            .FirstOrDefaultAsync(r => r.StudentId == studentId);

        if (resume == null)
            return Result<ResumeSearchDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)));

        return Result<ResumeSearchDto>.Success(ResumeMapper.ToSearchDto(resume, maskContacts: false));
    }

    public async Task<Result<ResumeFillDto>> UpdateResumeAsync(Guid studentId, ResumeFillDto dto)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.StudyPlan)
            .Include(s => s.Resume).ThenInclude(r => r!.AdditionalSkills)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student?.Resume == null)
            return Result<ResumeFillDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)));

        ResumeMapper.ApplyUpdate(student.Resume, dto);
        _registrationManager.RecalculateRegistrationStage(student);

        var result = await SaveChangesAsync<Student>();
        return result.IsSuccess
            ? Result<ResumeFillDto>.Success(ResumeMapper.ToFillDto(student.Resume))
            : Result<ResumeFillDto>.Failure(result.ErrorMessage!);
    }

    public Task<Result<bool>> SoftDeleteResumeAsync(Guid studentId) => SoftDeleteResumeInternalAsync(studentId);
    public Task<Result<bool>> RestoreResumeAsync(Guid studentId) => RestoreResumeInternalAsync(studentId);

    public async Task<Result<bool>> HardDeleteResumeAsync(Guid studentId)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.Resume)
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student?.Resume == null) return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)));

        _context.Resumes.Remove(student.Resume);

        student.Resume = null;
        _registrationManager.RecalculateRegistrationStage(student);

        return (await SaveChangesAsync<Student>()).IsSuccess
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("Ошибка удаления");
    }
}