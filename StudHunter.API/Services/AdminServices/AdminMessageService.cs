using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.MessageDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing messages with administrative privileges.
/// </summary>
public class AdminMessageService(StudHunterDbContext context) : BaseMessageService(context)
{
    /// <summary>
    /// Retrieves all messages.
    /// </summary>
    /// <returns>A tuple containing a list of all messages, an optional status code, and an optional error message.</returns>
    public async Task<(List<MessageDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllMessagesAsync()
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Invitation)
            .Select(m => MapToMessageDto(m))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return (messages, null, null);
    }

    /// <summary>
    /// Retrieves sent messages by user Id.
    /// </summary>
    /// <returns>A tuple containing a list of sent user messages, an optional status code, and an optional error message.</returns>
    public async Task<(List<MessageDto>? Entities, int? StatusCode, string? ErrorMessage)> GetMessagesByUserAsync(Guid userId, bool sent)
    {
        var messageQuery = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Invitation)
            .Include(m => m.Chat)
            .Where(m => sent ? m.SenderId == userId : (m.Chat.User1Id == userId || m.Chat.User2Id == userId));

        var messages = await messageQuery
            .Select(m => MapToMessageDto(m))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return (messages, null, null);
    }

    /// <summary>
    /// Deletes a message.
    /// </summary>
    /// <param name="messageId">The unique identifier (GUID) of the message.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteMessageAsync(Guid messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Message)));

        _context.Messages.Remove(message);
        return await SaveChangesAsync<Message>();
    }
}
