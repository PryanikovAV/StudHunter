using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminBlackListService
{
    Task<Result<PagedResult<BlackListDto>>> GetUserBlackListAsync(Guid targetUserId, PaginationParams paging);
}

public class AdminBlackListService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BlackListService(context, registrationManager), IAdminBlackListService
{
    public async Task<Result<PagedResult<BlackListDto>>> GetUserBlackListAsync(Guid targetUserId, PaginationParams paging)
    {
        var query = _context.BlackLists
            .AsNoTracking()
            .Include(b => b.BlockedUser)
            .Where(b => b.UserId == targetUserId);

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
}