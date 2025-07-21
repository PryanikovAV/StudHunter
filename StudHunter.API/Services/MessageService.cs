using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class MessageService(StudHunterDbContext context) : BaseEntityService(context)
{
    public async Task<IEnumerable<MessageDto>> GetMessagesByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Messages.Where(m => m.SenderId == userId)
        : _context.Messages.Where(m => m.ReceiverId == userId);

        return await query
        .Include(m => m.Sender)
        .Include(m => m.Receiver)
        .Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderEmail = m.Sender.IsDeleted ? "[Удалённый аккаунт]" : m.Sender.Email,
            ReceiverId = m.ReceiverId,
            ReceiverEmail = m.Receiver.IsDeleted ? "[Удалённый аккаунт]" : m.Receiver.Email,
            Context = m.Context,
            SentAt = m.SentAt
        })
        .OrderByDescending(m => m.SentAt)
        .ToListAsync();
    }

    public async Task<(MessageDto? Message, string? Error)> CreateMessageAsync(Guid senderId, CreateMessageDto dto)
    {
        if (senderId == dto.ReceiverId)
            return (null, "Sender and receiver cannot be the same.");

        if (!await _context.Users.AnyAsync(u => u.Id == senderId))
            return (null, "Sender not found.");

        if (!await _context.Users.AnyAsync(u => u.Id == dto.ReceiverId))
            return (null, "Receiver not found.");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Context = dto.Context,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);

        var (success, error) = await SaveChangesAsync("create", "Message");
        if (!success)
            return (null, error);

        return (new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Context = message.Context,
            SentAt = message.SentAt
        }, null);
    }
}
