using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AuthService;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IEmployerService
{
    Task<Result<EmployerDto>> GetEmployerAsync(Guid employerId);
    Task<Result<EmployerDto>> UpdateEmployerAsync(Guid employerId, UpdateEmployerDto dto);
    Task<Result<bool>> DeleteEmployerAsync(Guid employerId);
}

public class EmployerService(StudHunterDbContext context, IPasswordHasher passwordHasher)
    : BaseEmployerService(context), IEmployerService
{
    public async Task<Result<EmployerDto>> GetEmployerAsync(Guid employerId)
    {
        var employer = await GetEmployerInternalAsync(employerId);

        if (employer == null)
            return Result<EmployerDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        return Result<EmployerDto>.Success(EmployerMapper.ToDto(employer));
    }

    public async Task<Result<EmployerDto>> UpdateEmployerAsync(Guid employerId, UpdateEmployerDto dto)
    {
        var employer = await GetEmployerInternalAsync(employerId);

        if (employer == null)
            return Result<EmployerDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        EmployerMapper.ApplyUpdate(employer, dto);

        if (!string.IsNullOrWhiteSpace(dto.Password))
            employer.PasswordHash = passwordHasher.HashPassword(dto.Password);

        RecalculateRegistrationStage(employer);

        var result = await SaveChangesAsync<Employer>();

        return result.IsSuccess
            ? Result<EmployerDto>.Success(EmployerMapper.ToDto(employer))
            : Result<EmployerDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> DeleteEmployerAsync(Guid employerId)
    {
        var employer = await GetEmployerInternalAsync(employerId, ignoreFilters: true);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        if (employer.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.EntityAlreadyDeleted(nameof(Employer)), StatusCodes.Status400BadRequest);

        var now = DateTime.UtcNow;

        await SoftDeleteEmployerAsync(employer, now);

        return await SaveChangesAsync<Employer>();
    }

}