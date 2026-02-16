using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseBlackListService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected IQueryable<BlackList> GetBaseBlackListQuery(Guid userId) =>
        _context.BlackLists
            .AsNoTracking()
            .Include(b => b.BlockedUser)
            .Where(b => b.UserId == userId);

    protected async Task ClearCommunicationBetweenUsersAsync(Guid user1Id, Guid user2Id)
    {
        var now = DateTime.UtcNow;

        var favoritesToRemove = await _context.Favorites
            .Include(f => f.Vacancy)
            .Include(f => f.Resume)
            .Where(f =>
                (f.UserId == user1Id && (f.EmployerId == user2Id || (f.Vacancy != null && f.Vacancy.EmployerId == user2Id) || (f.Resume != null && f.Resume.StudentId == user2Id)))
                ||
                (f.UserId == user2Id && (f.EmployerId == user1Id || (f.Vacancy != null && f.Vacancy.EmployerId == user1Id) || (f.Resume != null && f.Resume.StudentId == user1Id)))
            )
            .ToListAsync();

        if (favoritesToRemove.Any()) _context.Favorites.RemoveRange(favoritesToRemove);

        var invitations = await _context.Invitations
            .Where(i => ((i.SenderId == user1Id && i.ReceiverId == user2Id) ||
                         (i.SenderId == user2Id && i.ReceiverId == user1Id))
                     && (i.Status == Invitation.InvitationStatus.Sent || i.Status == Invitation.InvitationStatus.Accepted))
            .ToListAsync();

        foreach (var inv in invitations)
        {
            inv.Status = Invitation.InvitationStatus.Rejected;
            inv.UpdatedAt = now;
        }
    }
}