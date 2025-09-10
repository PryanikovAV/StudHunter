using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.MessageDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

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

    public async Task<(List<MessageDto>? Entities, int? StatusCode, string? ErrorMessage)> GetMessagesByChatAsync(Guid authUserId, Guid chatId)
    {
        var chat = await _context.Chats
            .Where(c => c.Id == chatId && (c.User1Id == authUserId || c.User2Id == authUserId))
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

    public async Task<(MessageDto? Entity, int? StatusCode, string? ErrorMessage)> GetMessageByIdAsync(Guid authUserId, Guid messageId)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Invitation)
            .Include(m => m.Chat)
            .FirstOrDefaultAsync(m => m.Id == messageId && (m.Chat.User1Id == authUserId || m.Chat.User2Id == authUserId));

        if (message == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Message)));

        return (MapToMessageDto(message), null, null);
    }

    /// <summary>
    /// Creates a new message.
    /// </summary>
    /// <param name="senderId">The unique identifier (GUID) of the sender.</param>
    /// <param name="dto">The data transfer object containing message details.</param>
    /// <returns>A tuple containing the created message, an optional status code, and an optional error message.</returns>
    public async Task<(MessageDto? Entity, int? StatusCode, string? ErrorMessage)> CreateMessageAsync(Guid senderId, CreateMessageDto dto)
    {
        if (senderId == dto.ReceiverId)
            return (null, StatusCodes.Status400BadRequest, "Sender and receiver cannot be the same.");

        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("sender"));

        var receiver = await _context.Users.FindAsync(dto.ReceiverId);
        if (receiver == null || receiver.IsDeleted)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("receiver"));

        if ((sender is Student && receiver is Student) || (sender is Employer && receiver is Employer))
            return (null, StatusCodes.Status400BadRequest, $"{nameof(Message)} can only be sent between {nameof(Student)} and {nameof(Employer)} or with an admin.");

        var user1Id = senderId < dto.ReceiverId ? senderId : dto.ReceiverId;
        var user2Id = senderId < dto.ReceiverId ? dto.ReceiverId : senderId;

        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.User1Id == user1Id && c.User2Id == user2Id);

        if (chat == null)
        {
            chat = new Chat
            {
                Id = Guid.NewGuid(),
                User1Id = user1Id,
                User2Id = user2Id,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };
            _context.Chats.Add(chat);
        }
        else
        {
            chat.LastMessageAt = DateTime.UtcNow;
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chat.Id,
            SenderId = senderId,
            Content = dto.Content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Message>();
        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToMessageDto(message), null, null);
    }
}
