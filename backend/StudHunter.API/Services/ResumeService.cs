using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IResumeService
{
    Task<Result<ResumeDto>> GetResumeByStudentIdAsync(Guid studentId, Guid? currentUserId = null, bool ignoreFilters = true);
    Task<Result<ResumeDto>> CreateResumeAsync(Guid studentId, UpdateResumeDto dto);
    Task<Result<ResumeDto>> UpdateResumeAsync(Guid studentId, UpdateResumeDto dto);
    Task<Result<bool>> SoftDeleteResumeAsync(Guid studentId);
    Task<Result<ResumeDto>> RestoreResumeAsync(Guid studentId);
    Task<Result<PagedResult<ResumeDto>>> SearchResumesAsync(ResumeSearchFilter filter, Guid? currentUserId = null);
}

public class ResumeService(StudHunterDbContext context) : BaseResumeService(context), IResumeService
{
    public async Task<Result<ResumeDto>> CreateResumeAsync(Guid studentId, UpdateResumeDto dto)
    {
        var student = await _context.Users.FindAsync(studentId);
        if (student == null)
            return Result<ResumeDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)));

        var permission = EnsureCanPerform(student, UserAction.CreateResume);
        if (!permission.IsSuccess)
            return Result<ResumeDto>.Failure(permission.ErrorMessage!, permission.StatusCode);

        if (await _context.Resumes.AnyAsync(r => r.StudentId == studentId))
            return Result<ResumeDto>.Failure(ErrorMessages.AlreadyExists(nameof(Resume)));

        var resume = new Resume
        {
            StudentId = studentId,
            Title = dto.Title?.Trim() ?? "Новое резюме",
            Description = dto.Description?.Trim()
        };

        if (dto.SkillIds?.Any() == true)
            UpdateResumeSkills(resume, dto.SkillIds);

        _context.Resumes.Add(resume);

        var result = await SaveChangesAsync<Resume>();

        return result.IsSuccess
            ? await GetResumeByStudentIdAsync(studentId, null, true)
            : Result<ResumeDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<ResumeDto>> UpdateResumeAsync(Guid studentId, UpdateResumeDto dto)
    {
        var resume = await _context.Resumes
            .Include(r => r.AdditionalSkills)
            .FirstOrDefaultAsync(r => r.StudentId == studentId);

        if (resume == null)
            return Result<ResumeDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)), StatusCodes.Status404NotFound);

        ResumeMapper.ApplyUpdate(resume, dto);
        if (dto.SkillIds != null)
            UpdateResumeSkills(resume, dto.SkillIds);

        var result = await SaveChangesAsync<Resume>();
        return result.IsSuccess
            ? await GetResumeByStudentIdAsync(studentId, null, true)
            : Result<ResumeDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<PagedResult<ResumeDto>>> SearchResumesAsync(ResumeSearchFilter filter, Guid? currentUserId = null)
    {
        if (currentUserId.HasValue)
        {
            var currentUser = await _context.Users.FindAsync(currentUserId.Value);
            var permission = EnsureCanPerform(currentUser!, UserAction.ViewResumes);
            if (!permission.IsSuccess)
                return Result<PagedResult<ResumeDto>>.Failure(permission.ErrorMessage!, permission.StatusCode);
        }

        var query = GetFullResumeQuery();
        query = query.Where(r => r.Student.RegistrationStage == User.AccountStatus.FullyActivated);

        if (currentUserId.HasValue)
        {
            var blockedIds = await GetBlockedUserIdsAsync(currentUserId.Value);
            if (blockedIds.Any())
                query = query.Where(r => !blockedIds.Contains(r.StudentId));
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(r => r.Title.ToLower().Contains(term)
            || (r.Description != null && r.Description.ToLower().Contains(term)));
        }

        if (filter.SkillIds?.Any() == true)
            query = query.Where(r => r.AdditionalSkills.Any(s => filter.SkillIds.Contains(s.AdditionalSkillId)));

        var pagedEntities = await query
            .OrderByDescending(r => r.UpdatedAt)
            .ToPagedResultAsync(filter.Paging);

        bool maskContacts = false;
        if (currentUserId.HasValue)
        {
            var user = await _context.Users.FindAsync(currentUserId.Value);
            maskContacts = !UserPermissions.IsAllowed(UserRoles.GetRole(user!), user!.RegistrationStage, UserAction.ViewContacts);
        }

        var dtos = pagedEntities.Items
            .Select(r => ResumeMapper.ToDto(r, false, maskContacts))
            .ToList();

        var results = new PagedResult<ResumeDto>(
            Items: dtos,
            TotalCount: pagedEntities.TotalCount,
            PageNumber: pagedEntities.PageNumber,
            PageSize: pagedEntities.PageSize
        );

        return Result<PagedResult<ResumeDto>>.Success(results);
    }
}

public record ResumeSearchFilter(
    string? SearchTerm = null,
    List<Guid>? SkillIds = null,
    PaginationParams Paging = null!
)
{
    public PaginationParams Paging { get; init; } = Paging ?? new PaginationParams();
};