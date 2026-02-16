using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminInvitationService : IInvitationService
{
    Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForUserAsync(Guid targetUserId, InvitationSearchFilter filter);
    Task<Result<bool>> HardDeleteAsync(Guid id);
    Task<Result<InvitationCardDto>> UpdateStatusForcedAsync(Guid id, Invitation.InvitationStatus status);
}
public class AdminInvitationService(StudHunterDbContext context,
    INotificationService notificationService,
    IRegistrationManager registrationManager)
    : InvitationService(context, notificationService, registrationManager), IAdminInvitationService
{
    public async Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForUserAsync(Guid targetUserId, InvitationSearchFilter filter)
    {
        var query = GetFullInvitationQuery();

        if (filter.Incoming)
            query = query.Where(i => i.ReceiverId == targetUserId);
        else
            query = query.Where(i => i.SenderId == targetUserId);

        if (filter.Status.HasValue)
            query = query.Where(i => i.Status == filter.Status);

        if (filter.Type.HasValue)
            query = query.Where(i => i.Type == filter.Type);

        query = query.OrderByDescending(i => i.CreatedAt);

        var pagedInvitations = await query.ToPagedResultAsync(filter.Paging);

        var dtos = pagedInvitations.Items
            .Select(i => InvitationMapper.ToCardDto(i, targetUserId))
            .ToList();

        var result = new PagedResult<InvitationCardDto>(
            Items: dtos,
            TotalCount: pagedInvitations.TotalCount,
            PageNumber: pagedInvitations.PageNumber,
            PageSize: pagedInvitations.PageSize
        );

        return Result<PagedResult<InvitationCardDto>>.Success(result);
    }

    public async Task<Result<bool>> HardDeleteAsync(Guid id)
    {
        var invitation = await _context.Invitations.FindAsync(id);
        if (invitation == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        _context.Invitations.Remove(invitation);
        return await SaveChangesAsync<Invitation>();
    }

    public async Task<Result<InvitationCardDto>> UpdateStatusForcedAsync(Guid id, Invitation.InvitationStatus status)
    {
        var invitation = await _context.Invitations
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Vacancy).ThenInclude(v => v!.Employer)
            .Include(i => i.Resume).ThenInclude(r => r!.Student).ThenInclude(s => s!.StudyPlan)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invitation == null)
            return Result<InvitationCardDto>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        invitation.Status = status;
        invitation.UpdatedAt = DateTime.UtcNow;

        var saveResult = await SaveChangesAsync<Invitation>();

        if (!saveResult.IsSuccess)
            return Result<InvitationCardDto>.Failure(saveResult.ErrorMessage!);

        return Result<InvitationCardDto>.Success(InvitationMapper.ToCardDto(invitation, invitation.SenderId));
    }
}