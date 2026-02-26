using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
// TODO: Add paging
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
        bool isStudent = await _context.Students.AnyAsync(s => s.Id == targetUserId);

        return isStudent
            ? await GetInvitationsForStudentAsync(targetUserId, filter)
            : await GetInvitationsForEmployerAsync(targetUserId, filter);
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
        var invitation = await GetFullInvitationQuery().FirstOrDefaultAsync(i => i.Id == id);
        
        if (invitation == null)
            return Result<InvitationCardDto>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        invitation.Status = status;
        invitation.UpdatedAt = DateTime.UtcNow;

        var saveResult = await SaveChangesAsync<Invitation>();
        
        if (!saveResult.IsSuccess)
            return Result<InvitationCardDto>.Failure(saveResult.ErrorMessage!);

        return Result<InvitationCardDto>.Success(InvitationMapper.ToCardDto(invitation, invitation.StudentId));
    }
}