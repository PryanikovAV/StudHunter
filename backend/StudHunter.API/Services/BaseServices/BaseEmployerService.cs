using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseEmployerService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected IQueryable<Employer> GetEmployerQuery(
        bool asNoTracking = false,
        bool ignoreFilters = false,
        bool includeOrganizationDetails = false,
        bool includeVacancies = false)
    {
        var query = _context.Employers.AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();
        
        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        query = query
            .Include(e => e.City)
            .Include(e => e.Specialization);

        if (includeOrganizationDetails)
            query = query.Include(e => e.OrganizationDetails);

        if (includeVacancies)
            query = query.Include(e => e.Vacancies);

        return query;
    }

    protected async Task<(HashSet<Guid> FavoriteEmployerIds, HashSet<Guid> BlockedEmployerIds)> GetEmployerUiFlagsAsync(Guid? currentUserId, List<Employer> employers)
    {
        if (!currentUserId.HasValue || !employers.Any())
            return (new HashSet<Guid>(), new HashSet<Guid>());

        var employerIds = employers.Select(e => e.Id).ToList();

        var favoriteEmployerIds = await _context.Favorites
            .Where(f => f.UserId == currentUserId.Value && f.EmployerId.HasValue && employerIds.Contains(f.EmployerId.Value))
            .Select(f => f.EmployerId!.Value)
            .ToHashSetAsync();

        var blockedEmployerIds = await _context.BlackLists
            .Where(b => b.UserId == currentUserId.Value && employerIds.Contains(b.BlockedUserId))
            .Select(b => b.BlockedUserId)
            .ToHashSetAsync();

        return (favoriteEmployerIds, blockedEmployerIds);
    }

    protected async Task SoftDeleteEmployerAsync(Employer employer, DateTime now)
    {
        employer.IsDeleted = true;
        employer.DeletedAt = now;

        foreach (var v in employer.Vacancies.Where(v => !v.IsDeleted))
        {
            v.IsDeleted = true;
            v.DeletedAt = now;
        }

        await ClearUserActivityAsync(employer.Id, now);

        _registrationManager.RecalculateRegistrationStage(employer);
    }
}
