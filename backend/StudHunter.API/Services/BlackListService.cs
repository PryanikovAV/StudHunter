using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IBlackListService
{
    Task<Result<bool>> ToggleBlockAsync(Guid userId, Guid blockedUserId, bool shouldBlock);
    Task<Result<PagedResult<BlackListDto>>> GetMyBlackListAsync(Guid userId, PaginationParams paging);
}

public class BlackListService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager), IBlackListService
{
    public async Task<Result<PagedResult<BlackListDto>>> GetMyBlackListAsync(Guid userId, PaginationParams paging)
    {
        var query = _context.BlackLists
            .AsNoTracking()
            .Include(b => b.BlockedUser)
            .Where(b => b.UserId == userId);

        var pagedBlacklist = await query
            .OrderByDescending(b => b.BlockedAt)
            .ToPagedResultAsync(paging ?? new PaginationParams());

        var dtos = pagedBlacklist.Items.Select(BlackListMapper.ToDto).ToList();

        var pagedResult = new PagedResult<BlackListDto>(
            Items: dtos,
            TotalCount: pagedBlacklist.TotalCount,
            PageNumber: pagedBlacklist.PageNumber,
            PageSize: pagedBlacklist.PageSize);

        return Result<PagedResult<BlackListDto>>.Success(pagedResult);
    }

    public async Task<Result<bool>> ToggleBlockAsync(Guid userId, Guid blockedUserId, bool shouldBlock)
    {
        var me = await _context.Users.FindAsync(userId);
        var target = await _context.Users.FindAsync(blockedUserId);

        if (me == null || target == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(User)), StatusCodes.Status404NotFound);

        var validation = ValidateBlockAction(me, target);
        if (!validation.IsSuccess)
            return validation;

        var existing = await _context.BlackLists
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BlockedUserId == blockedUserId);

        if (shouldBlock)
        {
            if (existing != null) return Result<bool>.Success(true);

            _context.BlackLists.Add(new BlackList { UserId = userId, BlockedUserId = blockedUserId });

            await ClearCommunicationBetweenUsersAsync(userId, blockedUserId);
        }
        else
        {
            if (existing == null) return Result<bool>.Success(true);

            _context.BlackLists.Remove(existing);
        }

        var result = await SaveChangesAsync<BlackList>();

        return result.IsSuccess
            ? Result<bool>.Success(shouldBlock)
            : Result<bool>.Failure(result.ErrorMessage!);
    }

    private Result<bool> ValidateBlockAction(User me, User target)
    {
        if (me.Id == target.Id)
            return Result<bool>.Failure("Нельзя заблокировать самого себя.");

        string myRole = UserRoles.GetRole(me);
        string targetRole = UserRoles.GetRole(target);

        if (targetRole == UserRoles.Administrator)
            return Result<bool>.Failure("Нельзя заблокировать администратора.", StatusCodes.Status403Forbidden);

        if (myRole == targetRole)
            return Result<bool>.Failure($"Вы можете блокировать только пользователей противоположной роли ({targetRole}).");

        return Result<bool>.Success(true);
    }

    private async Task ClearCommunicationBetweenUsersAsync(Guid user1Id, Guid user2Id)
    {
        var now = DateTime.UtcNow;

        var favoritesToRemove = await _context.Favorites
            .Include(f => f.Vacancy)
            .Where(f =>
                (f.UserId == user1Id && (f.EmployerId == user2Id || (f.Vacancy != null && f.Vacancy.EmployerId == user2Id) || f.StudentId == user2Id))
                ||
                (f.UserId == user2Id && (f.EmployerId == user1Id || (f.Vacancy != null && f.Vacancy.EmployerId == user1Id) || f.StudentId == user1Id))
            )
            .ToListAsync();

        if (favoritesToRemove.Any()) _context.Favorites.RemoveRange(favoritesToRemove);

        var invitations = await _context.Invitations
            .Where(i => ((i.StudentId == user1Id && i.EmployerId == user2Id) ||
                         (i.StudentId == user2Id && i.EmployerId == user1Id))
                     && (i.Status == Invitation.InvitationStatus.Sent || i.Status == Invitation.InvitationStatus.Accepted))
            .ToListAsync();

        foreach (var inv in invitations)
        {
            inv.Status = Invitation.InvitationStatus.Rejected;
            inv.UpdatedAt = now;
        }
    }
}