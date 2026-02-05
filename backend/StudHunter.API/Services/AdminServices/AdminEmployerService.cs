using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminEmployerService
{
    Task<Result<List<AdminEmployerDto>>> GetAllEmployersAsync();
    Task<Result<AdminEmployerDto>> GetEmployerByIdAsync(Guid employerId);
    Task<Result<bool>> UpdateEmployerAsync(Guid employerId, UpdateEmployerDto dto);
    Task<Result<bool>> VerifyEmployerAsync(Guid employerId);
    Task<Result<bool>> DeleteEmployerAsync(Guid employerId, bool hardDelete);
    Task<Result<bool>> RestoreAsync(Guid employerId);
}

public class AdminEmployerService(StudHunterDbContext context) : BaseEmployerService(context), IAdminEmployerService
{
    public async Task<Result<List<AdminEmployerDto>>> GetAllEmployersAsync()
    {
        var employers = await _context.Employers
            .IgnoreQueryFilters()
            .Include(e => e.Vacancies)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        var dtos = employers.Select(e => EmployerMapper.ToAdminDto(e)).ToList();

        return Result<List<AdminEmployerDto>>.Success(dtos);
    }

    public async Task<Result<AdminEmployerDto>> GetEmployerByIdAsync(Guid employerId)
    {
        var employer = await GetEmployerInternalAsync(employerId, ignoreFilters: true);

        if (employer is null)
            return Result<AdminEmployerDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        return Result<AdminEmployerDto>.Success(EmployerMapper.ToAdminDto(employer));
    }

    public async Task<Result<bool>> UpdateEmployerAsync(Guid employerId, UpdateEmployerDto dto)
    {
        var employer = await GetEmployerInternalAsync(employerId, ignoreFilters: true);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        EmployerMapper.ApplyUpdate(employer, dto);

        RecalculateRegistrationStage(employer);

        return await SaveChangesAsync<Employer>();
    }

    public async Task<Result<bool>> VerifyEmployerAsync(Guid employerId)
    {
        var employer = await GetEmployerInternalAsync(employerId, ignoreFilters: true);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        employer.RegistrationStage = User.AccountStatus.FullyActivated;

        RecalculateRegistrationStage(employer);

        return await SaveChangesAsync<Employer>();
    }

    public async Task<Result<bool>> DeleteEmployerAsync(Guid employerId, bool hardDelete)
    {
        var employer = await GetEmployerInternalAsync(employerId, ignoreFilters: true);

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

    public async Task<Result<bool>> RestoreAsync(Guid employerId)
    {
        var employer = await GetEmployerInternalAsync(employerId, ignoreFilters: true);

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

        RecalculateRegistrationStage(employer);

        return await SaveChangesAsync<Employer>();
    }
}