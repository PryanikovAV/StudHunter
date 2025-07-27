using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Message;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminMessagesService(StudHunterDbContext context) : MessageService(context)
{
    public async Task<(List<MessageDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllMessageAsync()
    {
        var messages = await _context.Messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Context = m.Context,
            SentAt = m.SentAt
        })
        .OrderByDescending(m => m.SentAt)
        .ToListAsync();

        return (messages, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteMessageAsync(Guid id)
    {
        return await DeleteEntityAsync<Message>(id, hardDelete: true);
    }
}
