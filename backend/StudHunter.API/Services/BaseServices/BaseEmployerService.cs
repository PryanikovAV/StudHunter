using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseEmployerService(StudHunterDbContext context) : BaseService(context)
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

        RecalculateRegistrationStage(employer);
    }

    public static void RecalculateRegistrationStage(Employer employer)
    {
        if (employer.IsDeleted)
        {
            employer.RegistrationStage = User.AccountStatus.Anonymous;
            return;
        }

        if (employer.RegistrationStage == User.AccountStatus.Anonymous)
            return;

        bool isDataValid =
            employer.Name != UserDefaultNames.DefaultCompanyName &&
            !string.IsNullOrWhiteSpace(employer.ContactPhone) &&
            !string.IsNullOrWhiteSpace(employer.Description);

        if (isDataValid && employer.RegistrationStage == User.AccountStatus.FullyActivated)
            return;
        else
        {
            employer.RegistrationStage = User.AccountStatus.ProfileFilled;
            return;
        }            
    }
}
