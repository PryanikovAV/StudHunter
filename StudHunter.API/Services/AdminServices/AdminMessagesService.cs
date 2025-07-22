using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminMessagesService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<IEnumerable<MessageDto>> GetAllMessageAsync()
    {
        return await _context.Messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Context = m.Context,
            SentAt = m.SentAt
        })
        .OrderByDescending(m => m.SentAt)
        .ToListAsync();
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Messages.Where(m => m.SenderId == userId)
        : _context.Messages.Where(m => m.ReceiverId == userId);

        return await query.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Context = m.Context,
            SentAt = m.SentAt
        })
        .OrderByDescending(m => m.SentAt)
        .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> DeleteMessageAsync(Guid id)
    {
        return await HardDeleteEntityAsync<Message>(id);
    }
}
