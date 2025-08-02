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
public class MessageService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Retrieves messages for a specific user, either sent or received.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="sent">A boolean indicating whether to retrieve sent (true) or received (false) messages.</param>
    /// <returns>A tuple containing a list of messages, an optional status code, and an optional error message.</returns>
    public async Task<(List<MessageDto>? Entities, int? StatusCode, string? ErrorMessage)> GetMessagesByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Messages.Where(m => m.SenderId == userId)
        : _context.Messages.Where(m => m.ReceiverId == userId);

        var messages = await query
        .Include(m => m.Sender)
        .Include(m => m.Receiver)
        .Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderEmail = m.Sender.IsDeleted ? "[Deleted account]" : m.Sender.Email,
            ReceiverId = m.ReceiverId,
            ReceiverEmail = m.Receiver.IsDeleted ? "[Deleted account]" : m.Receiver.Email,
            Context = m.Context,
            SentAt = m.SentAt
        })
        .OrderByDescending(m => m.SentAt)
        .ToListAsync();

        return (messages, null, null);
    }

    /// <summary>
    /// Retrieves a message by its ID for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the message.</param>
    /// <param name="userId">The unique identifier (GUID) of the user (sender).</param>
    /// <returns>A tuple containing the message, an optional status code, and an optional error message.</returns>
    public async Task<(MessageDto? Entity, int? StatusCode, string? ErrorMessage)> GetMessageAsync(Guid id, Guid userId)
    {
        var message = await _context.Messages
        .Where(m => m.Id == id && m.SenderId == userId)
        .Include(m => m.Sender)
        .Include(m => m.Receiver)
        .Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderEmail = m.Sender.IsDeleted ? "[Deleted account]" : m.Sender.Email,
            ReceiverId = m.ReceiverId,
            ReceiverEmail = m.Receiver.IsDeleted ? "[Deleted account]" : m.Receiver.Email,
            Context = m.Context,
            SentAt = m.SentAt
        })
        .FirstOrDefaultAsync();

        #region Serializers
        if (message == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Message)));
        #endregion

        return (message, null, null);
    }

    /// <summary>
    /// Creates a new message.
    /// </summary>
    /// <param name="senderId">The unique identifier (GUID) of the sender.</param>
    /// <param name="dto">The data transfer object containing message details.</param>
    /// <returns>A tuple containing the created message, an optional status code, and an optional error message.</returns>
    public async Task<(MessageDto? Entity, int? StatusCode, string? ErrorMessage)> CreateMessageAsync(Guid senderId, CreateMessageDto dto)
    {
        #region Serializers
        if (senderId == dto.ReceiverId)
            return (null, StatusCodes.Status400BadRequest, "Sender and receiver cannot be the same.");

        if (!await _context.Users.AnyAsync(u => u.Id == senderId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("sender"));

        if (!await _context.Users.AnyAsync(u => u.Id == dto.ReceiverId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("receiver"));
        #endregion

        var message = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Context = dto.Context,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Message>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Context = message.Context,
            SentAt = message.SentAt
        }, null, null);
    }
}
