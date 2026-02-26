using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AuthService;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IEmployerService
{
    Task<Result<EmployerDto>> GetEmployerProfileAsync(Guid employerId);
    Task<Result<EmployerHeroDto>> GetEmployerHeroAsync(Guid employerId, Guid? currentUserId = null);
    Task<Result<EmployerDto>> UpdateEmployerProfileAsync(Guid employerId, UpdateEmployerDto dto);
    Task<Result<bool>> ChangePasswordAsync(Guid employerId, ChangePasswordDto dto);
    Task<Result<string>> UpdateAvatarAsync(Guid employerId, ChangeAvatarDto dto);
    Task<Result<bool>> DeleteEmployerAsync(Guid employerId, string password);
}

public class EmployerService(
    StudHunterDbContext context,
    IPasswordHasher passwordHasher,
    IRegistrationManager registrationManager)
    : BaseEmployerService(context, registrationManager), IEmployerService
{
    public async Task<Result<EmployerDto>> GetEmployerProfileAsync(Guid employerId)
    {
        var data = await GetEmployerQuery(asNoTracking: true, includeOrganizationDetails: true)
            .Where(e => e.Id == employerId)
            .Select(e => new
            {
                Employer = e,
                ActiveVacanciesCount = e.Vacancies.Count(v => !v.IsDeleted)
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return Result<EmployerDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        var dto = EmployerMapper.ToDto(data.Employer);

        var finalDto = dto with { VacanciesCount = data.ActiveVacanciesCount };

        return Result<EmployerDto>.Success(finalDto);
    }

    public async Task<Result<EmployerHeroDto>> GetEmployerHeroAsync(Guid employerId, Guid? currentUserId = null)
    {
        var employer = await GetEmployerQuery(asNoTracking: true, includeVacancies: true)
            .FirstOrDefaultAsync(e => e.Id == employerId && e.RegistrationStage == User.AccountStatus.FullyActivated);

        if (employer == null)
            return Result<EmployerHeroDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        var (favoriteIds, blackListIds) = await GetEmployerUiFlagsAsync(currentUserId, new List<Employer> { employer });

        return Result<EmployerHeroDto>.Success(EmployerMapper.ToHeroDto(
            employer,
            favoriteIds.Contains(employer.Id),
            blackListIds.Contains(employer.Id)
        ));
    }

    public async Task<Result<EmployerDto>> UpdateEmployerProfileAsync(Guid employerId, UpdateEmployerDto dto)
    {
        var employer = await GetEmployerQuery(includeOrganizationDetails: true)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<EmployerDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        EmployerMapper.ApplyUpdate(employer, dto);
        _registrationManager.RecalculateRegistrationStage(employer);

        var result = await SaveChangesAsync<Employer>();

        if (!result.IsSuccess)
            return Result<EmployerDto>.Failure(result.ErrorMessage!);

        return await GetEmployerProfileAsync(employerId);
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid employerId, ChangePasswordDto dto)
    {
        var employer = await GetEmployerQuery().FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        if (!passwordHasher.VerifyPassword(dto.CurrentPassword, employer.PasswordHash))
            return Result<bool>.Failure("Неверный текущий пароль", StatusCodes.Status400BadRequest);

        employer.PasswordHash = passwordHasher.HashPassword(dto.NewPassword);

        return await SaveChangesAsync<Employer>();
    }

    public async Task<Result<string>> UpdateAvatarAsync(Guid employerId, ChangeAvatarDto dto)
    {
        var employer = await GetEmployerQuery().FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<string>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        employer.AvatarUrl = dto.AvatarUrl;

        var result = await SaveChangesAsync<Employer>();

        return result.IsSuccess ? Result<string>.Success(employer.AvatarUrl) : Result<string>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> DeleteEmployerAsync(Guid employerId, string password)
    {
        var employer = await GetEmployerQuery(ignoreFilters: true, includeVacancies: true)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        if (employer.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.EntityAlreadyDeleted(nameof(Employer)), StatusCodes.Status400BadRequest);

        if (!passwordHasher.VerifyPassword(password, employer.PasswordHash))
            return Result<bool>.Failure(ErrorMessages.IncorrectPassword(), StatusCodes.Status400BadRequest);

        var now = DateTime.UtcNow;

        await SoftDeleteEmployerAsync(employer, now);

        return await SaveChangesAsync<Employer>();
    }
}