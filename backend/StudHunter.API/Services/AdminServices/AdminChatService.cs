using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminChatService : IChatService
{
    Task<Result<bool>> DeleteMessageAsync(Guid messageId);
}

public class AdminChatService(StudHunterDbContext context, IHubContext<ChatHub> chatHubContext, INotificationService notificationService)
    : ChatService(context, chatHubContext, notificationService), IAdminChatService
{
    public async Task<Result<bool>> DeleteMessageAsync(Guid messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);

        if (message == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Message)), StatusCodes.Status404NotFound);

        _context.Messages.Remove(message);

        return await SaveChangesAsync<Message>();
    }
}