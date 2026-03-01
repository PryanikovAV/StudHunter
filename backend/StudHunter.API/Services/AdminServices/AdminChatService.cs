using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminChatService
{
    Task<Result<PagedResult<ChatDto>>> GetUserChatsAsync(Guid targetUserId, PaginationParams paging);
    Task<Result<PagedResult<MessageDto>>> InspectChatMessagesAsync(Guid chatId, PaginationParams paging);
    Task<Result<bool>> DeleteMessageAsync(Guid messageId);
}

public class AdminChatService(StudHunterDbContext context,
    IHubContext<ChatHub> chatHubContext,
    INotificationService notificationService,
    IRegistrationManager registrationManager)
    : ChatService(context, chatHubContext, notificationService, registrationManager), IAdminChatService
{
    public async Task<Result<PagedResult<ChatDto>>> GetUserChatsAsync(Guid targetUserId, PaginationParams paging)
    {
        var pagedChats = await GetFullChatQuery()
            .Where(c => c.User1Id == targetUserId || c.User2Id == targetUserId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToPagedResultAsync(paging ?? new PaginationParams());

        var dtos = pagedChats.Items.Select(c => ChatMapper.ToDto(c, targetUserId, false, false)).ToList();

        return Result<PagedResult<ChatDto>>.Success(new PagedResult<ChatDto>(
            dtos, pagedChats.TotalCount, pagedChats.PageNumber, pagedChats.PageSize));
    }

    public async Task<Result<PagedResult<MessageDto>>> InspectChatMessagesAsync(Guid chatId, PaginationParams paging)
    {
        var chatExists = await _context.Chats.AnyAsync(c => c.Id == chatId);

        if (!chatExists)
            return Result<PagedResult<MessageDto>>.Failure(ErrorMessages.EntityNotFound(nameof(Chat)), StatusCodes.Status404NotFound);

        var pagedMessages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.SentAt)
            .ToPagedResultAsync(paging ?? new PaginationParams());

        var dtos = pagedMessages.Items.Select(ChatMapper.ToDto).ToList();

        return Result<PagedResult<MessageDto>>.Success(new PagedResult<MessageDto>(
            dtos, pagedMessages.TotalCount, pagedMessages.PageNumber, pagedMessages.PageSize));
    }

    public async Task<Result<bool>> DeleteMessageAsync(Guid messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);

        if (message == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Message)), StatusCodes.Status404NotFound);

        _context.Messages.Remove(message);
        var result = await SaveChangesAsync<Message>();

        return result.IsSuccess
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(result.ErrorMessage!);
    }
}