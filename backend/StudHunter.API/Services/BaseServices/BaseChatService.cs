using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseChatService(StudHunterDbContext context) : BaseService(context)
{
    protected (Guid user1, Guid user2) SortUserIds(Guid id1, Guid id2) =>
        id1.CompareTo(id2) < 0 ? (id1, id2) : (id2, id1);

    protected IQueryable<Chat> GetFullChatQuery() =>
        _context.Chats
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Include(c => c.Messages
            .OrderByDescending(m => m.SentAt).Take(1));

    protected async Task<Result<bool>> CanUsersChatAsync(Guid senderId, Guid receiverId)
    {
        /* TODO: читать чат с IsDeleted пользователм можно, добавить логику "писать нельзя" */
        var sender = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == senderId);
        var receiver = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == receiverId);

        if (sender == null || receiver == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(User)), StatusCodes.Status404NotFound);

        if (sender.IsDeleted || receiver.IsDeleted)
            return Result<bool>.Failure("Нельзя отправить сообщение удаленному пользователю.");

        if (GetRole(sender) == UserRoles.Administrator || GetRole(receiver) == UserRoles.Administrator)
            return Result<bool>.Success(true);

        if (GetRole(sender) == GetRole(receiver))
            return Result<bool>.Failure("Общение разрешено только между студентами и работодателями.");

        var stageSenderCheck = EnsureCanPerform(sender, UserAction.SendMessage);
        if (!stageSenderCheck.IsSuccess)
            return stageSenderCheck;

        var stageReceiverCheck = EnsureCanPerform(receiver, UserAction.SendMessage);
        if (!stageReceiverCheck.IsSuccess)
            return stageReceiverCheck;

        var blackListCheck = await EnsureCommunicationAllowedAsync(senderId, receiverId);
        if (!blackListCheck.IsSuccess)
            return blackListCheck;

        return Result<bool>.Success(true);
    }
}
