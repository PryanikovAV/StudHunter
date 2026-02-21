using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminEmployerService
{
    Task<Result<PagedResult<AdminEmployerDto>>> GetAllEmployersAsync(PaginationParams paging);
    Task<Result<AdminEmployerDto>> GetEmployerByIdAsync(Guid employerId);
    Task<Result<bool>> UpdateEmployerAsync(Guid employerId, UpdateEmployerDto dto);
    Task<Result<bool>> VerifyEmployerAsync(Guid employerId);
    Task<Result<bool>> DeleteEmployerAsync(Guid employerId, bool hardDelete);
    Task<Result<bool>> RestoreEmployerAsync(Guid employerId);
}

public class AdminEmployerService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseEmployerService(context, registrationManager), IAdminEmployerService
{
    public async Task<Result<PagedResult<AdminEmployerDto>>> GetAllEmployersAsync(PaginationParams paging)
    {
        var query = GetEmployerQuery(
            asNoTracking: true,
            ignoreFilters: true,
            includeOrganizationDetails: true,
            includeVacancies: true
        );

        var pagedEntities = await query.OrderByDescending(e => e.CreatedAt).ToPagedResultAsync(paging);

        var dtos = pagedEntities.Items.Select(EmployerMapper.ToAdminDto).ToList();

        return Result<PagedResult<AdminEmployerDto>>.Success(new PagedResult<AdminEmployerDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }

    public async Task<Result<AdminEmployerDto>> GetEmployerByIdAsync(Guid employerId)
    {
        var employer = await GetEmployerQuery(asNoTracking: true, ignoreFilters: true, includeOrganizationDetails: true, includeVacancies: true)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer is null)
            return Result<AdminEmployerDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        return Result<AdminEmployerDto>.Success(EmployerMapper.ToAdminDto(employer));
    }

    public async Task<Result<bool>> UpdateEmployerAsync(Guid employerId, UpdateEmployerDto dto)
    {
        var employer = await GetEmployerQuery(ignoreFilters: true, includeOrganizationDetails: true)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        EmployerMapper.ApplyUpdate(employer, dto);

        _registrationManager.RecalculateRegistrationStage(employer);

        return await SaveChangesAsync<Employer>();
    }

    public async Task<Result<bool>> VerifyEmployerAsync(Guid employerId)
    {
        var employer = await GetEmployerQuery(ignoreFilters: true)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        employer.RegistrationStage = User.AccountStatus.FullyActivated;

        _registrationManager.RecalculateRegistrationStage(employer);

        return await SaveChangesAsync<Employer>();
    }

    public async Task<Result<bool>> DeleteEmployerAsync(Guid employerId, bool hardDelete)
    {
        var employer = await GetEmployerQuery(ignoreFilters: true, includeVacancies: true)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        if (hardDelete)
        {
            _context.Employers.Remove(employer);
        }
        else
        {
            if (employer.IsDeleted)
                return Result<bool>.Failure(ErrorMessages.EntityAlreadyDeleted(nameof(Employer)), StatusCodes.Status400BadRequest);

            await SoftDeleteEmployerAsync(employer, DateTime.UtcNow);
        }

        return await SaveChangesAsync<Employer>();
    }

    public async Task<Result<bool>> RestoreEmployerAsync(Guid employerId)
    {
        var employer = await GetEmployerQuery(ignoreFilters: true, includeVacancies: true)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        if (!employer.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.AccountAlreadyActive(), StatusCodes.Status400BadRequest);

        var deletedAt = employer.DeletedAt;
        employer.IsDeleted = false;
        employer.DeletedAt = null;

        foreach (var v in employer.Vacancies.Where(v => v.DeletedAt >= deletedAt))
        {
            v.IsDeleted = false;
            v.DeletedAt = null;
        }

        _registrationManager.RecalculateRegistrationStage(employer);

        return await SaveChangesAsync<Employer>();
    }
}