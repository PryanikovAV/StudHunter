using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IChatService
{
    Task<Result<PagedResult<ChatDto>>> GetMyChatsAsync(Guid userId, PaginationParams paging);
    Task<Result<PagedResult<MessageDto>>> GetChatMessagesAsync(Guid userId, Guid chatId, PaginationParams paging);
    Task<Result<MessageDto>> SendMessageAsync(Guid senderId, Guid receiverId, string content, Guid? invitationId = null);
    Task<Result<bool>> MarkMessagesAsReadAsync(Guid userId, Guid chatId);
    Task<Result<ChatParticipantDto>> GetChatParticipantAsync(Guid interlocutorId);
}

public class ChatService(StudHunterDbContext context, 
    IHubContext<ChatHub> chatHubContext,
    INotificationService notificationService,
    IRegistrationManager registrationManager)
    : BaseChatService(context, registrationManager), IChatService
{
    private readonly IHubContext<ChatHub> _chatHubContext = chatHubContext;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<PagedResult<ChatDto>>> GetMyChatsAsync(Guid userId, PaginationParams paging)
    {
        var blockedByMe = await _context.BlackLists
            .Where(b => b.UserId == userId)
            .Select(b => b.BlockedUserId)
            .ToListAsync();

        var blockedByOthers = await _context.BlackLists
            .Where(b => b.BlockedUserId == userId)
            .Select(b => b.UserId)
            .ToListAsync();

        var pagedChats = await GetFullChatQuery()
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToPagedResultAsync(paging ?? new PaginationParams());

        var dtos = pagedChats.Items.Select(c =>
        {
            var interlocutorId = c.User1Id == userId ? c.User2Id : c.User1Id;
            bool isBlockedByMe = blockedByMe.Contains(interlocutorId);
            bool isBlockedByInterlocutor = blockedByOthers.Contains(interlocutorId);

            return ChatMapper.ToDto(c, userId, isBlockedByMe, isBlockedByInterlocutor);
        }).ToList();

        var pageResult = new PagedResult<ChatDto>(dtos, pagedChats.TotalCount, pagedChats.PageNumber, pagedChats.PageSize);

        return Result<PagedResult<ChatDto>>.Success(pageResult);
    }

    public async Task<Result<PagedResult<MessageDto>>> GetChatMessagesAsync(Guid userId, Guid chatId, PaginationParams paging)
    {
        var chatExists = await _context.Chats.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == chatId && (c.User1Id == userId || c.User2Id == userId));

        if (chatExists == null)
            return Result<PagedResult<MessageDto>>.Failure(ErrorMessages.EntityNotFound(nameof(Chat)), StatusCodes.Status404NotFound);

        var pagedMessages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.SentAt)
            .ToPagedResultAsync(paging ?? new PaginationParams());

        var dtos = pagedMessages.Items.Select(m => ChatMapper.ToDto(m)).ToList();

        var pageResult = new PagedResult<MessageDto>(
            Items: dtos,
            TotalCount: pagedMessages.TotalCount,
            PageNumber: pagedMessages.PageNumber,
            PageSize: pagedMessages.PageSize);

        return Result<PagedResult<MessageDto>>.Success(pageResult);
    }

    public async Task<Result<MessageDto>> SendMessageAsync(Guid senderId, Guid receiverId, string content, Guid? invitationId = null)
    {
        var canChatResult = await CanUsersChatAsync(senderId, receiverId);

        if (!canChatResult.IsSuccess)
            return Result<MessageDto>.Failure(canChatResult.ErrorMessage!, canChatResult.StatusCode);

        var (u1, u2) = SortUserIds(senderId, receiverId);
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.User1Id == u1 && c.User2Id == u2);

        if (chat == null)
        {
            chat = new Chat { User1Id = u1, User2Id = u2, CreatedAt = DateTime.UtcNow };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
        }

        var message = new Message
        {
            ChatId = chat.Id,
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content.Trim(),
            InvitationId = invitationId,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        chat.LastMessageAt = message.SentAt;
        _context.Messages.Add(message);

        var result = await SaveChangesAsync<Message>();

        if (result.IsSuccess)
        {
            var dto = ChatMapper.ToDto(message);

            await _chatHubContext.Clients
                .Group(message.ChatId.ToString())
                .SendAsync("ReceiveMessage", dto);

            var sender = await _context.Users.FindAsync(senderId);

            await _notificationService.SendAsync(
                userId: receiverId,
                title: $"Новое сообщение от {UserDisplayHelper.GetUserDisplayName(sender!)}",
                message: content.Length > 50 ? content[..50] + "..." : content,
                type: Notification.NotificationType.ChatMessage,
                entityId: chat.Id,
                senderId: senderId
            );
        }

        return result.IsSuccess
            ? Result<MessageDto>.Success(ChatMapper.ToDto(message))
            : Result<MessageDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<ChatParticipantDto>> GetChatParticipantAsync(Guid interlocutorId)
    {
        var user = await _context.Users.FindAsync(interlocutorId);

        if (user == null)
            return Result<ChatParticipantDto>.Failure(ErrorMessages.EntityNotFound(nameof(User)), StatusCodes.Status404NotFound);

        return Result<ChatParticipantDto>.Success(ChatMapper.ToParticipantDto(user));
    }

    public async Task<Result<bool>> MarkMessagesAsReadAsync(Guid userId, Guid chatId)
    {
        await _context.Messages
            .Where(m => m.ChatId == chatId && m.ReceiverId == userId && !m.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));

        return Result<bool>.Success(true);
    }
}