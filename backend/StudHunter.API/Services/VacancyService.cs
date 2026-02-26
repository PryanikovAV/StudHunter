using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IVacancyService
{
    Task<Result<VacancyFillDto>> GetVacancyByIdForEditAsync(Guid vacancyId, Guid employerId);
    Task<Result<PagedResult<VacancySearchDto>>> GetMyVacanciesAsync(Guid employerId, PaginationParams paging, bool includeDeleted);
    Task<Result<VacancySearchDto>> GetVacancyDetailsAsync(Guid vacancyId, Guid? currentUserId = null);
    Task<Result<VacancyFillDto>> CreateVacancyAsync(Guid employerId, VacancyFillDto dto);
    Task<Result<VacancyFillDto>> UpdateVacancyAsync(Guid vacancyId, Guid employerId, VacancyFillDto dto);
    Task<Result<bool>> SoftDeleteVacancyAsync(Guid vacancyId, Guid employerId);
    Task<Result<bool>> RestoreVacancyAsync(Guid vacancyId, Guid employerId);
    Task<Result<PagedResult<VacancySearchDto>>> SearchVacanciesAsync(VacancySearchFilter filter, Guid? currentUserId = null);
}

public class VacancyService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseVacancyService(context, registrationManager), IVacancyService
{
    public async Task<Result<VacancyFillDto>> GetVacancyByIdForEditAsync(Guid vacancyId, Guid employerId)
    {
        var vacancy = await GetVacancyQuery(asNoTracking: true, includeTags: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId && v.EmployerId == employerId);

        if (vacancy == null)
            return Result<VacancyFillDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        return Result<VacancyFillDto>.Success(VacancyMapper.ToFillDto(vacancy));
    }

    public async Task<Result<PagedResult<VacancySearchDto>>> GetMyVacanciesAsync(Guid employerId, PaginationParams paging, bool includeDeleted)
    {
        var query = GetVacancyQuery(asNoTracking: true, ignoreFilters: includeDeleted, includeEmployerData: true, includeTags: true)
            .Where(v => v.EmployerId == employerId);

        var pagedEntities = await query.OrderByDescending(v => v.CreatedAt).ToPagedResultAsync(paging);

        var dtos = pagedEntities.Items.Select(v => VacancyMapper.ToSearchDto(v)).ToList();

        return Result<PagedResult<VacancySearchDto>>.Success(new PagedResult<VacancySearchDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }

    public async Task<Result<VacancySearchDto>> GetVacancyDetailsAsync(Guid vacancyId, Guid? currentUserId = null)
    {
        var vacancy = await GetVacancyQuery(asNoTracking: true, includeEmployerData: true, includeTags: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null) return Result<VacancySearchDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        var (favIds, blkIds) = await GetVacancyUiFlagsAsync(currentUserId, new List<Vacancy> { vacancy });

        return Result<VacancySearchDto>.Success(VacancyMapper.ToSearchDto(
            vacancy,
            favIds.Contains(vacancy.Id),
            blkIds.Contains(vacancy.EmployerId)
        ));
    }

    public async Task<Result<VacancyFillDto>> CreateVacancyAsync(Guid employerId, VacancyFillDto dto)
    {
        var employer = await _context.Employers.Include(e => e.Vacancies).FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<VacancyFillDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)));

        var vacancy = new Vacancy { EmployerId = employerId, Title = dto.Title };
        _context.Vacancies.Add(vacancy);

        VacancyMapper.ApplyUpdate(vacancy, dto);

        _registrationManager.RecalculateRegistrationStage(employer);
        var result = await SaveChangesAsync<Vacancy>();

        return result.IsSuccess
            ? await GetVacancyByIdForEditAsync(vacancy.Id, employerId)
            : Result<VacancyFillDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<VacancyFillDto>> UpdateVacancyAsync(Guid vacancyId, Guid employerId, VacancyFillDto dto)
    {
        var vacancy = await GetVacancyQuery(includeTags: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId && v.EmployerId == employerId);

        if (vacancy == null)
            return Result<VacancyFillDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        VacancyMapper.ApplyUpdate(vacancy, dto);

        var result = await SaveChangesAsync<Vacancy>();

        return result.IsSuccess
            ? await GetVacancyByIdForEditAsync(vacancyId, employerId)
            : Result<VacancyFillDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> SoftDeleteVacancyAsync(Guid vacancyId, Guid employerId)
    {
        var vacancy = await GetVacancyQuery()
            .FirstOrDefaultAsync(v => v.Id == vacancyId && v.EmployerId == employerId);

        if (vacancy == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        vacancy.IsDeleted = true;
        vacancy.DeletedAt = DateTime.UtcNow;

        var employer = await _context.Employers.FirstAsync(e => e.Id == employerId);
        _registrationManager.RecalculateRegistrationStage(employer);

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResult<VacancySearchDto>>> SearchVacanciesAsync(VacancySearchFilter filter, Guid? currentUserId = null)
    {
        var query = GetVacancyQuery(asNoTracking: true, includeEmployerData: true, includeTags: true)
            .Where(v => !v.IsDeleted && !v.Employer.IsDeleted && v.Employer.RegistrationStage == User.AccountStatus.FullyActivated);

        if (filter.EmployerId.HasValue)
        {
            query = query.Where(v => v.EmployerId == filter.EmployerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = $"%{filter.SearchTerm.Trim()}%";
            query = query.Where(v =>
                EF.Functions.ILike(v.Title, term) ||
                (v.Description != null && EF.Functions.ILike(v.Description, term)) ||
                EF.Functions.ILike(v.Employer.Name, term));
        }

        if (filter.CourseIds?.Any() == true)
            query = query.Where(v => v.Courses.Any(c => filter.CourseIds.Contains(c.CourseId)));

        if (filter.SkillIds?.Any() == true)
            query = query.Where(v => v.AdditionalSkills.Any(s => filter.SkillIds.Contains(s.AdditionalSkillId)));

        if (!string.IsNullOrWhiteSpace(filter.VacancyType) && Enum.TryParse<Vacancy.VacancyType>(filter.VacancyType, out var type))
            query = query.Where(v => v.Type == type);

        var pagedEntities = await query.OrderByDescending(v => v.UpdatedAt).ToPagedResultAsync(filter.Paging);

        var (favIds, blkIds) = await GetVacancyUiFlagsAsync(currentUserId, pagedEntities.Items);

        var dtos = pagedEntities.Items.Select(v => VacancyMapper.ToSearchDto(
            v,
            favIds.Contains(v.Id),
            blkIds.Contains(v.EmployerId)
        )).ToList();

        return Result<PagedResult<VacancySearchDto>>.Success(new PagedResult<VacancySearchDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }

    public async Task<Result<bool>> RestoreVacancyAsync(Guid vacancyId, Guid employerId)
    {
        var vacancy = await GetVacancyQuery(ignoreFilters: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId && v.EmployerId == employerId);

        if (vacancy == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        if (!vacancy.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.AlreadyExists(nameof(Vacancy)), StatusCodes.Status400BadRequest);

        vacancy.IsDeleted = false;
        vacancy.DeletedAt = null;

        var employer = await _context.Employers.FirstAsync(e => e.Id == employerId);
        _registrationManager.RecalculateRegistrationStage(employer);

        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}