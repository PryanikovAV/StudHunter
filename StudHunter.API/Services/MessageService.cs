using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class MessageService(StudHunterDbContext context) : BaseService(context)
{
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
            SenderEmail = m.Sender.IsDeleted ? "[Удалённый аккаунт]" : m.Sender.Email,
            ReceiverId = m.ReceiverId,
            ReceiverEmail = m.Receiver.IsDeleted ? "[Удалённый аккаунт]" : m.Receiver.Email,
            Context = m.Context,
            SentAt = m.SentAt
        })
        .OrderByDescending(m => m.SentAt)
        .ToListAsync();

        return (messages, null, null);
    }

    public async Task<(MessageDto? Entity, int? StatusCode, string? ErrorMessage)> CreateMessageAsync(Guid senderId, CreateMessageDto dto)
    {
        #region Serializers
        if (senderId == dto.ReceiverId)
            return (null, StatusCodes.Status400BadRequest, "Sender and receiver cannot be the same.");

        var senderExists = await _context.Users.AnyAsync(u => u.Id == senderId);
        if (senderExists == false)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Sender"));

        var receiverExists = await _context.Users.AnyAsync(u => u.Id == dto.ReceiverId);
        if (receiverExists == false)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Receiver"));
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

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Message");

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
