using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseEmployerService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected async Task<Employer?> GetEmployerInternalAsync(Guid employerId, bool ignoreFilters = false)
    {
        var query = _context.Employers.AsQueryable();

        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        return await query
            .Include(e => e.Vacancies)
            .Include(e => e.OrganizationDetails)
            .Include(e => e.City)
            .FirstOrDefaultAsync(e => e.Id == employerId);
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
