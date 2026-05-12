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
    Task<Result<PagedResult<VacancySearchDto>>> GetVacanciesByEmployerAsync(Guid employerId, PaginationParams paging, bool includeDeleted);
    Task<Result<VacancySearchDto>> GetVacancyDetailsAsync(Guid vacancyId, Guid? currentUserId = null);
    Task<Result<VacancyFillDto>> CreateVacancyAsync(Guid employerId, VacancyFillDto dto);
    Task<Result<VacancyFillDto>> UpdateVacancyAsync(Guid vacancyId, Guid employerId, VacancyFillDto dto);
    Task<Result<bool>> SoftDeleteVacancyAsync(Guid vacancyId, Guid employerId);
    Task<Result<bool>> RestoreVacancyAsync(Guid vacancyId, Guid employerId);
    Task<Result<HomePageDto>> GetHomePageDataAsync(Guid? currentUserId = null);
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

    public async Task<Result<PagedResult<VacancySearchDto>>> GetVacanciesByEmployerAsync(
        Guid employerId,
        PaginationParams paging,
        bool includeDeleted = false)
    {
        var query = GetVacancyQuery(
                asNoTracking: true,
                ignoreFilters: includeDeleted,
                includeEmployerData: true,
                includeTags: true)
            .Where(v => v.EmployerId == employerId);

        if (!includeDeleted)
        {
            query = query.Where(v => v.Employer.RegistrationStage == User.AccountStatus.FullyActivated);
        }

        var projectedQuery = query.Select(v => new
        {
            Vacancy = v,
            Total = v.Invitations.Count(i => i.Type == Invitation.InvitationType.Response),
            Active = v.Invitations.Count(i => i.Type == Invitation.InvitationType.Response && i.Status == Invitation.InvitationStatus.Sent)
        });

        var pagedEntities = await projectedQuery
            .OrderByDescending(x => x.Vacancy.CreatedAt)
            .ToPagedResultAsync(paging);

        var dtos = pagedEntities.Items.Select(item =>
        {
            var baseDto = VacancyMapper.ToSearchDto(item.Vacancy);
            return baseDto with { TotalResponses = item.Total, ActiveResponses = item.Active };
        }).ToList();

        return Result<PagedResult<VacancySearchDto>>.Success(new PagedResult<VacancySearchDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }

    public async Task<Result<VacancySearchDto>> GetVacancyDetailsAsync(Guid vacancyId, Guid? currentUserId = null)
    {
        var vacancy = await GetVacancyQuery(
            asNoTracking: true,
            includeEmployerData: true,
            includeTags: true)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);

        if (vacancy == null)
            return Result<VacancySearchDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)));

        if (vacancy.Employer.RegistrationStage != User.AccountStatus.FullyActivated && vacancy.EmployerId != currentUserId)
        {
            return Result<VacancySearchDto>.Failure(
                "Вакансия временно недоступна: профиль работодателя находится на проверке.",
                StatusCodes.Status403Forbidden);
        }

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

    public async Task<Result<HomePageDto>> GetHomePageDataAsync(Guid? currentUserId = null)
    {
        var baseQuery = GetVacancyQuery(asNoTracking: true, includeEmployerData: true, includeTags: true)
            .Where(v => !v.IsDeleted && !v.Employer.IsDeleted && v.Employer.RegistrationStage == User.AccountStatus.FullyActivated);

        if (currentUserId.HasValue)
        {
            var blockedEmployerIds = await _context.BlackLists
                .Where(b => b.UserId == currentUserId.Value)
                .Select(b => b.BlockedUserId)
                .ToListAsync();

            if (blockedEmployerIds.Any())
                baseQuery = baseQuery.Where(v => !blockedEmployerIds.Contains(v.EmployerId));
        }

        var categories = new List<CategoryCardDto>();

        var internshipsCount = await baseQuery.CountAsync(v => v.Type == Vacancy.VacancyType.Internship);
        categories.Add(new CategoryCardDto("Стажировки и практики", internshipsCount, "VacancyTypes", Vacancy.VacancyType.Internship.ToString()));


        var jobsCount = await baseQuery.CountAsync(v => v.Type == Vacancy.VacancyType.Job);
        categories.Add(new CategoryCardDto("Первая работа", jobsCount, "VacancyTypes", Vacancy.VacancyType.Job.ToString()));

        // ТОП-2 самые популярные Specialization
        var topSpecializations = await baseQuery
            .Where(v => v.Employer.SpecializationId != null)
            .GroupBy(v => new { v.Employer.SpecializationId, v.Employer.Specialization!.Name })
            .Select(g => new
            {
                SpecId = g.Key.SpecializationId,
                Name = g.Key.Name,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(2)
            .ToListAsync();

        foreach (var spec in topSpecializations)
        {
            categories.Add(new CategoryCardDto(spec.Name, spec.Count, "SpecializationIds", spec.SpecId.ToString()!));
        }

        var latestEntities = await baseQuery
            .OrderByDescending(v => v.CreatedAt)
            .Take(8)
            .ToListAsync();

        var (favIds, blkIds) = await GetVacancyUiFlagsAsync(currentUserId, latestEntities);
        var latestDtos = latestEntities.Select(v => VacancyMapper.ToSearchDto(
            v,
            favIds.Contains(v.Id),
            blkIds.Contains(v.EmployerId)
        )).ToList();

        return Result<HomePageDto>.Success(new HomePageDto(categories, latestDtos));
    }
}