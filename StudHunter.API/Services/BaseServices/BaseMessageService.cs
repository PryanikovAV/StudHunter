using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Message;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using StudHunter.API.ModelsDto.Chat;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseMessageService(StudHunterDbContext context) : BaseService(context)
{
    protected static MessageDto MapToMessageDto(Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            SenderEmail = message.Sender.IsDeleted ? "[Deleted account]" : message.Sender.Email,
            Content = message.Content,
            InvitationId = message.InvitationId,
            SentAt = message.SentAt
        };
    }

    protected static ChatDto MapToChatDto(Chat chat)
    {
        return new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.User1Id,
            User1Email = chat.User1.IsDeleted ? "[Deleted Account]" : chat.User1.Email,
            User2Id = chat.User2Id,
            User2Email = chat.User2.IsDeleted ? "[Deleted Account]" : chat.User2.Email,
            CreatedAt = chat.CreatedAt,
            LastMessageAt = chat.LastMessageAt
        };
    }

    public async Task<(List<MessageDto>? Entities, int? StatusCode, string? ErrorMessage)> GetMessagesByChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _context.Chats
        .Where(c => c.Id == chatId && (c.User1Id == userId || c.User2Id == userId))
        .FirstOrDefaultAsync();

        if (chat == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Chat)));

        var messages = await _context.Messages
        .Where(m => m.ChatId == chatId)
        .Include(m => m.Sender)
        .Include(m => m.Invitation)
        .Select(m => MapToMessageDto(m))
        .OrderBy(m => m.SentAt)
        .ToListAsync();

        return (messages, null, null);
    }

    public async Task<(List<ChatDto>? Entities, int? StatusCode, string? ErrorMessage)> GetChatsByUserAsync(Guid userId)
    {
        var chats = await _context.Chats
        .Where(c => c.User1Id == userId || c.User2Id == userId)
        .Select(c => MapToChatDto(c))
        .OrderByDescending(c => c.LastMessageAt)
        .ToListAsync();

        return (chats, null, null);
    }
}
