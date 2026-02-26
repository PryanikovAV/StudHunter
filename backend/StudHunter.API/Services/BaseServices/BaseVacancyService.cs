using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseVacancyService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected IQueryable<Vacancy> GetVacancyQuery(
        bool asNoTracking = false,
        bool ignoreFilters = false,
        bool includeEmployerData = false,
        bool includeTags = false)
    {
        var query = _context.Vacancies.AsQueryable();

        if (asNoTracking) query = query.AsNoTracking();
        if (ignoreFilters) query = query.IgnoreQueryFilters();

        if (includeEmployerData)
        {
            query = query
                .Include(v => v.Employer).ThenInclude(e => e!.City)
                .Include(v => v.Employer).ThenInclude(e => e!.Specialization)
                .Include(v => v.Employer).ThenInclude(e => e!.OrganizationDetails);
        }

        if (includeTags)
        {
            query = query
                .Include(v => v.Courses).ThenInclude(vc => vc.Course)
                .Include(v => v.AdditionalSkills).ThenInclude(vas => vas.AdditionalSkill);
        }

        return query;
    }

    protected async Task<(HashSet<Guid> FavoriteVacancyIds, HashSet<Guid> BlockedEmployerIds)> GetVacancyUiFlagsAsync(Guid? currentUserId, List<Vacancy> vacancies)
    {
        if (!currentUserId.HasValue || !vacancies.Any())
            return (new HashSet<Guid>(), new HashSet<Guid>());

        var vacancyIds = vacancies.Select(v => v.Id).ToList();
        var employerIds = vacancies.Select(v => v.EmployerId).ToList();

        var favoriteVacancyIds = await _context.Favorites
            .Where(f => f.UserId == currentUserId.Value && f.VacancyId.HasValue && vacancyIds.Contains(f.VacancyId.Value))
            .Select(f => f.VacancyId!.Value)
            .ToHashSetAsync();

        var blockedEmployerIds = await _context.BlackLists
            .Where(b => b.UserId == currentUserId.Value && employerIds.Contains(b.BlockedUserId))
            .Select(b => b.BlockedUserId)
            .ToHashSetAsync();

        return (favoriteVacancyIds, blockedEmployerIds);
    }
}