using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
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
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Vacancy)
                .ThenInclude(v => v!.Employer)
            .Include(i => i.Resume)
                .ThenInclude(r => r!.Student)
                    .ThenInclude(s => s!.StudyPlan)
                        .ThenInclude(sp => sp!.University)
            .Include(i => i.Resume)
                .ThenInclude(r => r!.Student)
                    .ThenInclude(s => s.StudyPlan)
                        .ThenInclude(sp => sp!.StudyDirection)
            .Include(i => i.Resume)
                .ThenInclude(r => r!.AdditionalSkills)
                    .ThenInclude(ras => ras.AdditionalSkill);

    protected async Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsInternalAsync(Guid userId, InvitationSearchFilter filter)
    {
        var blockedIds = await GetBlockedUserIdsAsync(userId);
        var query = GetFullInvitationQuery();

        if (filter.Incoming)
            query = query.Where(i => i.ReceiverId == userId && !blockedIds.Contains(i.SenderId));
        else
            query = query.Where(i => i.SenderId == userId);

        if (filter.Status.HasValue)
            query = query.Where(i => i.Status == filter.Status);

        if (filter.Type.HasValue)
            query = query.Where(i => i.Type == filter.Type);

        query = query.OrderByDescending(i => i.CreatedAt);

        var pagedInvitations = await query.ToPagedResultAsync(filter.Paging);

        var dtos = pagedInvitations.Items
            .Select(i => InvitationMapper.ToCardDto(i, userId))
            .ToList();

        var pagedResult = new PagedResult<InvitationCardDto>(
            Items: dtos,
            TotalCount: pagedInvitations.TotalCount,
            PageNumber: pagedInvitations.PageNumber,
            PageSize: pagedInvitations.PageSize
        );

        return Result<PagedResult<InvitationCardDto>>.Success(pagedResult);
    }

    protected async Task PopulateSnapshotsAsync(Invitation invitation)
    {
        if (invitation.SnapshotSenderName == null)
        {
            var sender = await _context.Users.FindAsync(invitation.SenderId);
            if (sender != null)
                invitation.SnapshotSenderName = UserDisplayHelper.GetUserDisplayName(sender);
        }

        if (invitation.VacancyId.HasValue && invitation.SnapshotVacancyTitle == null)
        {
            var vacancy = await _context.Vacancies.FindAsync(invitation.VacancyId.Value);
            invitation.SnapshotVacancyTitle = vacancy?.Title;
        }

        if (invitation.SnapshotReceiverName == null)
        {
            var receiver = await _context.Users.FindAsync(invitation.ReceiverId);
            if (receiver != null)
                invitation.SnapshotReceiverName = UserDisplayHelper.GetUserDisplayName(receiver);
        }
    }
}
