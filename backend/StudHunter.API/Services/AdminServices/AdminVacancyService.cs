using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;
// TODO: добавить пагинацию
public interface IAdminVacancyService
{
    Task<Result<List<VacancyDto>>> GetAllVacanciesAsync(Guid employerId);
    Task<Result<VacancyDto>> UpdateVacancyAsync(Guid vacancyId, UpdateVacancyDto dto);
    Task<Result<bool>> HardDeleteVacancyAsync(Guid vacancyId);
    Task<Result<bool>> SoftDeleteVacancyAsync(Guid vacancyId);
    Task<Result<VacancyDto>> RestoreVacancyAsync(Guid vacancyId);
}

public class AdminVacancyService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : VacancyService(context, registrationManager), IAdminVacancyService
{
    public async Task<Result<List<VacancyDto>>> GetAllVacanciesAsync(Guid employerId)
    {
        var vacancies = await GetFullVacancyQuery()
            .IgnoreQueryFilters()
            .Where(v => v.EmployerId == employerId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return Result<List<VacancyDto>>.Success(vacancies.Select(v => VacancyMapper.ToDto(v)).ToList());
    }

    public async Task<Result<VacancyDto>> UpdateVacancyAsync(Guid vacancyId, UpdateVacancyDto dto)
    {
        var vacancy = await GetFullVacancyQuery()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<VacancyDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        VacancyMapper.ApplyUpdate(vacancy, dto);
        UpdateRelatedEntities(vacancy, dto.SkillIds, dto.CourseIds);

        var result = await SaveChangesAsync<Vacancy>();
        return result.IsSuccess ? await GetVacancyById(vacancyId, ignoreFilters: true) : Result<VacancyDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> HardDeleteVacancyAsync(Guid vacancyId)
    {
        var vacancy = await _context.Vacancies.IgnoreQueryFilters().FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null) return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        _context.Vacancies.Remove(vacancy);
        var result = await SaveChangesAsync<Vacancy>();
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> SoftDeleteVacancyAsync(Guid vacancyId)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        vacancy.IsDeleted = true;
        vacancy.DeletedAt = DateTime.UtcNow;

        var result = await SaveChangesAsync<Vacancy>();
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<VacancyDto>> RestoreVacancyAsync(Guid vacancyId)
    {
        var vacancy = await _context.Vacancies.IgnoreQueryFilters().FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<VacancyDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        vacancy.IsDeleted = false;
        vacancy.DeletedAt = null;

        var result = await SaveChangesAsync<Vacancy>();
        return result.IsSuccess ? await GetVacancyById(vacancyId) : Result<VacancyDto>.Failure(result.ErrorMessage!);
    }
}