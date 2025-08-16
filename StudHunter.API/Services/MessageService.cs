using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing messages.
/// </summary>
public class MessageService(StudHunterDbContext context) : BaseMessageService(context)
{
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
            return (null, StatusCodes.Status400BadRequest, $"{nameof(Message)} can only sent between {nameof(Student)} and {nameof(Employer)}");

        var chat = await _context.Chats
        .FirstOrDefaultAsync(c => (c.User1Id == senderId && c.User2Id == dto.ReceiverId) || (c.User1Id == dto.ReceiverId && c.User2Id == senderId));

        if (chat == null)
        {
            chat = new Chat
            {
                Id = Guid.NewGuid(),
                User1Id = senderId,
                User2Id = dto.ReceiverId,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = null
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

    public async Task<(MessageDto? Entity, int? StatusCode, string? ErrorMessage)> GetMessageAsync(Guid id, Guid userId)
    {
        var message = await _context.Messages
        .Include(m => m.Sender)
        .Include(m => m.Invitation)
        .Include(m => m.Chat)
        .FirstOrDefaultAsync(m => m.Id == id && (m.Chat.User1Id == userId || m.Chat.User2Id == userId));

        if (message == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Message)));

        return (MapToMessageDto(message), null, null);
    }
}
