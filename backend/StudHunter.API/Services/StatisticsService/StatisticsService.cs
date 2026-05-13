using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.StatisticsService;

public interface IStatisticsService
{
    Task<GeneralStatisticsDto> GetGeneralStatisticsAsync(CancellationToken cancellationToken = default);
}

public class StatisticsService(StudHunterDbContext context) : IStatisticsService
{
    public async Task<GeneralStatisticsDto> GetGeneralStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalResumes = await context.Resumes.CountAsync(cancellationToken);
        var totalVacancies = await context.Vacancies.CountAsync(cancellationToken);
        var accreditedEmployers = await context.Employers
            .Where(e => e.RegistrationStage == User.AccountStatus.FullyActivated)
            .CountAsync(cancellationToken);

        return new GeneralStatisticsDto(
            totalResumes,
            totalVacancies,
            accreditedEmployers
        );
    }
}