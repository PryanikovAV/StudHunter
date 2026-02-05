using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IBlackListService
{
    Task<Result<bool>> ToggleBlockAsync(Guid userId, Guid blockedUserId, bool shouldBlock);
    Task<Result<PagedResult<BlackListDto>>> GetMyBlackListAsync(Guid userId, PaginationParams paging);
}

public class BlackListService(StudHunterDbContext context) : BaseBlackListService(context), IBlackListService
{
    public async Task<Result<PagedResult<BlackListDto>>> GetMyBlackListAsync(Guid userId, PaginationParams paging)
    {
        var pagedBlacklist = await GetBaseBlackListQuery(userId)
            .OrderByDescending(b => b.BlockedAt)
            .ToPagedResultAsync(paging);

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
            if (existing != null)
                return Result<bool>.Success(true);

            _context.BlackLists.Add(new BlackList { UserId = userId, BlockedUserId = blockedUserId });

            await ClearCommunicationBetweenUsersAsync(userId, blockedUserId);
        }
        else
        {
            if (existing == null)
                return Result<bool>.Success(true);

            _context.BlackLists.Remove(existing);
        }

        var result = await SaveChangesAsync<BlackList>();

        return result.IsSuccess
            ? Result<bool>.Success(shouldBlock)
            : Result<bool>.Failure(result.ErrorMessage!);
    }

    protected Result<bool> ValidateBlockAction(User me, User target)
    {
        if (me.Id == target.Id)
            return Result<bool>.Failure("Нельзя заблокировать самого себя.");

        if (GetRole(target) == UserRoles.Administrator)
            return Result<bool>.Failure("Нельзя заблокировать администратора.", StatusCodes.Status403Forbidden);

        if (GetRole(me) == GetRole(target))
            return Result<bool>.Failure($"Вы можете блокировать только пользователей противоположной роли ({GetRole(target)}).");

        return Result<bool>.Success(true);
    }
}