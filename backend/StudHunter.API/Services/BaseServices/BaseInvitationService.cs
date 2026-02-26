using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseInvitationService(StudHunterDbContext context, 
    INotificationService notificationService,
    IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected readonly INotificationService _notificationService = notificationService;

    protected IQueryable<Invitation> GetFullInvitationQuery() =>
        _context.Invitations
            .AsNoTracking()
            .Include(i => i.Student)
                .ThenInclude(s => s!.StudyPlan)
                    .ThenInclude(sp => sp!.University)
            .Include(i => i.Student)
                .ThenInclude(s => s!.StudyPlan)
                    .ThenInclude(sp => sp!.StudyDirection)
            .Include(i => i.Employer)
            .Include(i => i.Vacancy)
            .Include(i => i.Resume)
                .ThenInclude(r => r!.AdditionalSkills)
                    .ThenInclude(ras => ras.AdditionalSkill);

    protected async Task PopulateSnapshotsAsync(Invitation invitation)
    {
        if (invitation.SnapshotStudentName == null)
        {
            var student = await _context.Students.FindAsync(invitation.StudentId);
            if (student != null)
                invitation.SnapshotStudentName = UserDisplayHelper.GetUserDisplayName(student);
        }

        if (invitation.SnapshotEmployerName == null)
        {
            var employer = await _context.Employers.FindAsync(invitation.EmployerId);
            if (employer != null)
                invitation.SnapshotEmployerName = UserDisplayHelper.GetUserDisplayName(employer);
        }

        if (invitation.VacancyId.HasValue && invitation.SnapshotVacancyTitle == null)
        {
            var vacancy = await _context.Vacancies.FindAsync(invitation.VacancyId.Value);
            invitation.SnapshotVacancyTitle = vacancy?.Title;
        }
    }
}