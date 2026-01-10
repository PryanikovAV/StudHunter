using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public abstract class BaseService(StudHunterDbContext context)
{
    protected readonly StudHunterDbContext _context = context;

    protected Result<bool> EnsureCanPerform(User user, UserAction action)
    {
        var role = GetRole(user);

        if (UserPermissions.IsAllowed(role, user.RegistrationStage, action))
            return Result<bool>.Success(true);

        string message = UserPermissions.GetPermissionErrorMessage(role, user.RegistrationStage);

        return Result<bool>.Failure(message, StatusCodes.Status403Forbidden);
    }

    protected async Task<Result<bool>> EnsureCommunicationAllowedAsync(Guid user1Id, Guid user2Id)
    {
        var result = await _context.BlackLists.AsNoTracking()
            .AnyAsync(b => (b.UserId == user1Id && b.BlockedUserId == user2Id) ||
                           (b.UserId == user2Id && b.BlockedUserId == user1Id));
        if (result)
            return Result<bool>.Failure(ErrorMessages.CommunicationBlocked(), StatusCodes.Status403Forbidden);

        return Result<bool>.Success(true);
    }

    protected async Task<List<Guid>> GetBlockedUserIdsAsync(Guid currentUserId)
    {
        return await _context.BlackLists
            .AsNoTracking()
            .Where(b => b.UserId == currentUserId || b.BlockedUserId == currentUserId)
            .Select(b => b.UserId == currentUserId ? b.BlockedUserId : b.UserId)
            .ToListAsync();
    }

    /* При SoftDelete пользователя */
    protected async Task ClearUserActivityAsync(Guid userId, DateTime now)
    {
        var favorites = await _context.Favorites.Where(f => f.UserId == userId).ToListAsync();
        _context.Favorites.RemoveRange(favorites);

        var invitations = await _context.Invitations
            .Where(i => (i.SenderId == userId || i.ReceiverId == userId)
                     && (i.Status == Invitation.InvitationStatus.Sent || i.Status == Invitation.InvitationStatus.Accepted))
            .ToListAsync();

        foreach (var inv in invitations)
        {
            inv.Status = Invitation.InvitationStatus.Rejected;
            inv.UpdatedAt = now;
        }
    }

    protected string GetRole(User user) => UserRoles.GetRole(user);

    protected string GetUserDisplayName(User user) => user switch
    {
        Student s => $"{s.LastName} {s.FirstName}".Trim(),
        Employer e => e.Name,
        Administrator => UserRoles.Administrator,
        _ => user.Email ?? nameof(User)
    };

    protected async Task<Result<bool>> SaveChangesAsync<T>()
    {
        try
        {
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (DbUpdateException)
        {
            return Result<bool>.Failure(ErrorMessages.InvalidData(typeof(T).Name), StatusCodes.Status400BadRequest);
        }
    }
}
