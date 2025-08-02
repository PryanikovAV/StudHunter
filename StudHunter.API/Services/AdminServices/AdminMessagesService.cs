using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Message;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing messages with administrative privileges.
/// </summary>
public class AdminMessagesService(StudHunterDbContext context) : MessageService(context)
{
    /// <summary>
    /// Retrieves all messages.
    /// </summary>
    /// <returns>A tuple containing a list of all messages, an optional status code, and an optional error message.</returns>
    public async Task<(List<MessageDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllMessagesAsync()
    {
        var messages = await _context.Messages
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
    /// Deletes a message.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the message.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteMessageAsync(Guid id)
    {
        return await DeleteEntityAsync<Message>(id, hardDelete: true);
    }
}
