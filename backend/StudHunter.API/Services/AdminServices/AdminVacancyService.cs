using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminVacancyService
{
    Task<Result<PagedResult<VacancySearchDto>>> GetAllVacanciesAsync(PaginationParams paging);
    Task<Result<VacancySearchDto>> GetVacancyByIdAsync(Guid vacancyId);
    Task<Result<VacancyFillDto>> UpdateVacancyAsync(Guid vacancyId, VacancyFillDto dto);
    Task<Result<bool>> DeleteVacancyAsync(Guid vacancyId, bool hardDelete);
    Task<Result<bool>> RestoreVacancyAsync(Guid vacancyId);
}

public class AdminVacancyService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseVacancyService(context, registrationManager), IAdminVacancyService
{
    public async Task<Result<PagedResult<VacancySearchDto>>> GetAllVacanciesAsync(PaginationParams paging)
    {
        var query = GetVacancyQuery(asNoTracking: true, ignoreFilters: true, includeEmployerData: true, includeTags: true);

        var pagedEntities = await query.OrderByDescending(v => v.CreatedAt).ToPagedResultAsync(paging);
        var dtos = pagedEntities.Items.Select(VacancyMapper.ToSearchDto).ToList();

        return Result<PagedResult<VacancySearchDto>>.Success(new PagedResult<VacancySearchDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }

    public async Task<Result<VacancySearchDto>> GetVacancyByIdAsync(Guid vacancyId)
    {
        var vacancy = await GetVacancyQuery(asNoTracking: true, ignoreFilters: true, includeEmployerData: true, includeTags: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<VacancySearchDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        return Result<VacancySearchDto>.Success(VacancyMapper.ToSearchDto(vacancy));
    }

    public async Task<Result<VacancyFillDto>> UpdateVacancyAsync(Guid vacancyId, VacancyFillDto dto)
    {
        var vacancy = await GetVacancyQuery(ignoreFilters: true, includeTags: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<VacancyFillDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        VacancyMapper.ApplyUpdate(vacancy, dto);

        await _context.SaveChangesAsync();

        return Result<VacancyFillDto>.Success(VacancyMapper.ToFillDto(vacancy));
    }

    public async Task<Result<bool>> DeleteVacancyAsync(Guid vacancyId, bool hardDelete)
    {
        var vacancy = await GetVacancyQuery(ignoreFilters: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        if (hardDelete)
        {
            _context.Vacancies.Remove(vacancy);
        }
        else
        {
            if (vacancy.IsDeleted) return Result<bool>.Failure(ErrorMessages.EntityAlreadyDeleted(nameof(Vacancy)), StatusCodes.Status400BadRequest);
            vacancy.IsDeleted = true;
            vacancy.DeletedAt = DateTime.UtcNow;
        }

        var employer = await _context.Employers.FirstAsync(e => e.Id == vacancy.EmployerId);

        _registrationManager.RecalculateRegistrationStage(employer);

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RestoreVacancyAsync(Guid vacancyId)
    {
        var vacancy = await GetVacancyQuery(ignoreFilters: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        if (!vacancy.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.AlreadyExists(nameof(Vacancy)), StatusCodes.Status400BadRequest);

        vacancy.IsDeleted = false;
        vacancy.DeletedAt = null;

        var employer = await _context.Employers.FirstAsync(e => e.Id == vacancy.EmployerId);
        _registrationManager.RecalculateRegistrationStage(employer);

        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}