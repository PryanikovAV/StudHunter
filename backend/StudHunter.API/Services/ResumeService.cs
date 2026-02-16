using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IResumeService
{
    Task<Result<ResumeFillDto>> GetMyResumeAsync(Guid studentId);
    Task<Result<ResumeSearchDto>> GetResumeForEmployerAsync(Guid studentId, Guid currentUserId);
    Task<Result<ResumeFillDto>> UpsertResumeAsync(Guid studentId, ResumeFillDto dto);
    Task<Result<bool>> SoftDeleteResumeAsync(Guid studentId);
    Task<Result<PagedResult<ResumeSearchDto>>> SearchResumesAsync(ResumeSearchFilter filter, Guid? currentUserId = null);
}

public class ResumeService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseResumeService(context, registrationManager), IResumeService
{
    public async Task<Result<ResumeFillDto>> GetMyResumeAsync(Guid studentId)
    {
        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.AdditionalSkills)
            .ThenInclude(ras => ras.AdditionalSkill)
            .FirstOrDefaultAsync(r => r.StudentId == studentId && !r.IsDeleted);

        return Result<ResumeFillDto>.Success(ResumeMapper.ToFillDto(resume));
    }

    public async Task<Result<ResumeSearchDto>> GetResumeForEmployerAsync(Guid studentId, Guid currentUserId)
    {
        var resume = await GetFullResumeQuery()
            .FirstOrDefaultAsync(r => r.StudentId == studentId && !r.IsDeleted && r.Student.RegistrationStage == User.AccountStatus.FullyActivated);

        if (resume == null)
            return Result<ResumeSearchDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)));

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId);
        bool maskContacts = user == null || !UserPermissions.IsAllowed(UserRoles.GetRole(user), user.RegistrationStage, UserAction.ViewContacts);

        return Result<ResumeSearchDto>.Success(ResumeMapper.ToSearchDto(resume, maskContacts));
    }

    public async Task<Result<ResumeFillDto>> UpsertResumeAsync(Guid studentId, ResumeFillDto dto)
    {
        var student = await _context.Students
            .Include(s => s.StudyPlan)
            .Include(s => s.Resume).ThenInclude(r => r!.AdditionalSkills)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return Result<ResumeFillDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)));

        if (student.Resume == null)
        {
            student.Resume = new Resume { StudentId = studentId, Title = dto.Title };
            _context.Resumes.Add(student.Resume);
        }
        else if (student.Resume.IsDeleted)
        {
            student.Resume.IsDeleted = false;
            student.Resume.DeletedAt = null;
        }

        ResumeMapper.ApplyUpdate(student.Resume, dto);
        _registrationManager.RecalculateRegistrationStage(student);

        var result = await SaveChangesAsync<Student>();
        if (!result.IsSuccess)
            return Result<ResumeFillDto>.Failure(result.ErrorMessage!);

        return await GetMyResumeAsync(studentId);
    }

    public Task<Result<bool>> SoftDeleteResumeAsync(Guid studentId) => SoftDeleteResumeInternalAsync(studentId);

    public async Task<Result<PagedResult<ResumeSearchDto>>> SearchResumesAsync(ResumeSearchFilter filter, Guid? currentUserId = null)
    {
        var query = GetFullResumeQuery()
            .Where(r => !r.IsDeleted && !r.Student.IsDeleted && r.Student.RegistrationStage == User.AccountStatus.FullyActivated);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = $"%{filter.SearchTerm.Trim()}%";
            query = query.Where(r => EF.Functions.ILike(r.Title, term) || (r.Description != null && EF.Functions.ILike(r.Description, term)));
        }

        if (filter.SkillIds?.Any() == true)
        {
            query = query.Where(r => r.AdditionalSkills.Any(s => filter.SkillIds.Contains(s.AdditionalSkillId)));
        }

        var pagedEntities = await query.OrderByDescending(r => r.UpdatedAt).ToPagedResultAsync(filter.Paging);

        bool maskContacts = true;
        if (currentUserId.HasValue)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId.Value);
            if (user != null)
                maskContacts = !UserPermissions.IsAllowed(UserRoles.GetRole(user), user.RegistrationStage, UserAction.ViewContacts);
        }

        var dtos = pagedEntities.Items.Select(r => ResumeMapper.ToSearchDto(r, maskContacts)).ToList();

        return Result<PagedResult<ResumeSearchDto>>.Success(new PagedResult<ResumeSearchDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }
}