using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IVacancyService
{
    Task<Result<VacancyDto>> GetVacancyById(Guid id, Guid? currentUserId = null, bool ignoreFilters = false);
    Task<Result<PagedResult<VacancyDto>>> GetAllVacanciesAsync(Guid employerId, PaginationParams paging, bool ignoreFilters = false);
    Task<Result<VacancyDto>> CreateVacancyAsync(Guid employerId, UpdateVacancyDto dto);
    Task<Result<VacancyDto>> UpdateVacancyAsync(Guid vacancyId, UpdateVacancyDto dto, Guid authorizedEmployerId);
    Task<Result<bool>> SoftDeleteVacancyAsync(Guid vacancyId, Guid authorizedEmployerId);
    Task<Result<VacancyDto>> RestoreVacancyAsync(Guid vacancyId, Guid authorizedEmployerId);
    Task<Result<PagedResult<VacancyDto>>> SearchVacancyAsync(VacancySearchFilter filter, Guid? currentUserId = null);
}

public class VacancyService(StudHunterDbContext context) : BaseVacancyService(context), IVacancyService
{
    public async Task<Result<VacancyDto>> UpdateVacancyAsync(Guid vacancyId, UpdateVacancyDto dto, Guid authorizedEmployerId)
    {
        var employer = await _context.Employers
            .FirstOrDefaultAsync(e => e.Id == authorizedEmployerId);

        if (employer == null)
            return Result<VacancyDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        if (employer.RegistrationStage != User.AccountStatus.FullyActivated)
            return Result<VacancyDto>.Failure("Создание вакансий доступно только после аккредитации университета.", StatusCodes.Status403Forbidden);

        var vacancy = await _context.Vacancies
            .Include(v => v.AdditionalSkills)
            .Include(v => v.Courses)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<VacancyDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        if (vacancy.EmployerId != authorizedEmployerId)
            return Result<VacancyDto>.Failure(ErrorMessages.RestrictProfileActions("update", nameof(Vacancy)), StatusCodes.Status403Forbidden);

        VacancyMapper.ApplyUpdate(vacancy, dto);

        if (dto.SkillIds != null || dto.CourseIds != null)
            UpdateRelatedEntities(vacancy, dto.SkillIds, dto.CourseIds);

        var result = await SaveChangesAsync<Vacancy>();

        return result.IsSuccess
            ? await GetVacancyById(vacancyId)
            : Result<VacancyDto>.Failure(result.ErrorMessage!, result.StatusCode);
    }

    public async Task<Result<bool>> SoftDeleteVacancyAsync(Guid vacancyId, Guid authorizedEmployerId)
    {
        var vacancy = await _context.Vacancies
            .Include(v => v.Employer)
            .ThenInclude(e => e.Vacancies)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        if (vacancy.EmployerId != authorizedEmployerId)
            return Result<bool>.Failure(ErrorMessages.RestrictProfileActions("delete", nameof(Vacancy)), StatusCodes.Status403Forbidden);

        vacancy.IsDeleted = true;
        vacancy.DeletedAt = DateTime.UtcNow;

        BaseEmployerService.RecalculateRegistrationStage(vacancy.Employer);

        var result = await SaveChangesAsync<Vacancy>();

        return result.IsSuccess
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<VacancyDto>> RestoreVacancyAsync(Guid vacancyId, Guid authorizedEmployerId)
    {
        var vacancy = await _context.Vacancies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<VacancyDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        if (vacancy.EmployerId != authorizedEmployerId)
            return Result<VacancyDto>.Failure(ErrorMessages.RestrictProfileActions("restore", nameof(Vacancy)), StatusCodes.Status403Forbidden);

        vacancy.IsDeleted = false;
        vacancy.DeletedAt = null;

        await _context.SaveChangesAsync();
        return await GetVacancyById(vacancyId);
    }

    public async Task<Result<PagedResult<VacancyDto>>> SearchVacancyAsync(VacancySearchFilter filter, Guid? currentUserId = null)
    {
        var query = GetFullVacancyQuery();

        query = query.Where(v => v.Employer.RegistrationStage > User.AccountStatus.Anonymous);

        User.AccountStatus userStatus = User.AccountStatus.Anonymous;

        if (currentUserId.HasValue)
        {
            var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId.Value);
            userStatus = currentUser?.RegistrationStage ?? User.AccountStatus.Anonymous;

            var blockedIds = await GetBlockedUserIdsAsync(currentUserId.Value);
            if (blockedIds.Any())
                query = query.Where(v => !blockedIds.Contains(v.EmployerId));
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            string term = filter.SearchTerm.Trim().ToLower();
            query = query.Where(v => v.Title.ToLower().Contains(term)
                || (v.Description != null && v.Description.ToLower().Contains(term)));
        }

        if (filter.Type.HasValue)
            query = query.Where(v => v.Type == filter.Type.Value);

        if (filter.MinSalary.HasValue)
            query = query.Where(v => v.Salary >= filter.MinSalary.Value);

        if (filter.SkillIds?.Any() == true)
            query = query.Where(v => v.AdditionalSkills
                .Any(s => filter.SkillIds.Contains(s.AdditionalSkillId)));

        var pagedEntities = await query
            .OrderByDescending(v => v.CreatedAt)
            .ToPagedResultAsync(filter.Paging);
        
        var dtos = pagedEntities.Items
            .Select(v => VacancyMapper.ToDto(v, false, userStatus))
            .ToList();

        var result = new PagedResult<VacancyDto>(
            Items: dtos,
            TotalCount: pagedEntities.TotalCount,
            PageNumber: pagedEntities.PageNumber,
            PageSize: pagedEntities.PageSize);

        return Result<PagedResult<VacancyDto>>.Success(result);
    }
}

public record VacancySearchFilter(
    string? SearchTerm = null,
    Vacancy.VacancyType? Type = null,
    List<Guid>? SkillIds = null,
    decimal? MinSalary = null,
    PaginationParams Paging = null!
) {
    public PaginationParams Paging { get; init; } = Paging ?? new PaginationParams();
};
