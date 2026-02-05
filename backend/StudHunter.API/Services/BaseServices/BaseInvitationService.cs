using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseInvitationService(StudHunterDbContext context, INotificationService notificationService) : BaseService(context)
{
    protected readonly INotificationService _notificationService = notificationService;

    protected IQueryable<Invitation> GetFullInvitationQuery() =>
        _context.Invitations
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Vacancy)
            .Include(i => i.Resume);

    protected async Task PopulateSnapshotsAsync(Invitation invitation)
    {
        var sender = await _context.Users.FindAsync(invitation.SenderId);
        var receiver = await _context.Users.FindAsync(invitation.ReceiverId);

        if (sender != null)
            invitation.SnapshotSenderName = GetUserDisplayName(sender);

        if (receiver != null)
            invitation.SnapshotReceiverName = GetUserDisplayName(receiver);

        if (invitation.VacancyId.HasValue)
        {
            var vacancy = await _context.Vacancies.FindAsync(invitation.VacancyId.Value);
            invitation.SnapshotVacancyTitle = vacancy?.Title;
        }
    }
}
