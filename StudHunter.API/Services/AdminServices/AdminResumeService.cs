using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminResumeService
{
    Task<Result<ResumeDto>> GetResumeByStudentIdAsync(Guid studentId, Guid? currentUserId = null, bool ignoreFilters = true);
    Task<Result<ResumeDto>> UpdateResumeAsync(Guid studentId, UpdateResumeDto dto);
    Task<Result<bool>> HardDeleteResumeAsync(Guid studentId);
    Task<Result<bool>> SoftDeleteResumeAsync(Guid studentId);
    Task<Result<ResumeDto>> RestoreResumeAsync(Guid studentId);
}

public class AdminResumeService(StudHunterDbContext context) : BaseResumeService(context), IAdminResumeService
{
    public async Task<Result<ResumeDto>> UpdateResumeAsync(Guid studentId, UpdateResumeDto dto)
    {
        var resume = await GetFullResumeQuery()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.StudentId == studentId);

        if (resume == null)
            return Result<ResumeDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        ResumeMapper.ApplyUpdate(resume, dto);
        if (dto.SkillIds != null)
            UpdateResumeSkills(resume, dto.SkillIds);

        var result = await SaveChangesAsync<Resume>();
        return result.IsSuccess
            ? await GetResumeByStudentIdAsync(studentId, ignoreFilters: true)
            : Result<ResumeDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> HardDeleteResumeAsync(Guid studentId)
    {
        var resume = await _context.Resumes.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.StudentId == studentId);
        if (resume == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        _context.Resumes.Remove(resume);
        return (await SaveChangesAsync<Resume>()).IsSuccess
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("Ошибка удаления");
    }
}
