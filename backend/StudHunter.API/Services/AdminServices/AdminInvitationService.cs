using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminInvitationService : IInvitationService
{
    Task<Result<bool>> HardDeleteAsync(Guid id);
    Task<Result<InvitationDto>> UpdateStatusForcedAsync(Guid id, Invitation.InvitationStatus status);
}
public class AdminInvitationService(StudHunterDbContext context, INotificationService notificationService) : InvitationService(context, notificationService), IAdminInvitationService
{
    public async Task<Result<bool>> HardDeleteAsync(Guid id)
    {
        var invitation = await _context.Invitations.FindAsync(id);
        if (invitation == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        _context.Invitations.Remove(invitation);
        return await SaveChangesAsync<Invitation>();
    }

    public async Task<Result<InvitationDto>> UpdateStatusForcedAsync(Guid id, Invitation.InvitationStatus status)
    {
        var invitation = await GetFullInvitationQuery().FirstOrDefaultAsync(i => i.Id == id);
        if (invitation == null)
            return Result<InvitationDto>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        invitation.Status = status;
        invitation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Result<InvitationDto>.Success(InvitationMapper.ToDto(invitation));
    }
}