using StudHunter.API.Common;
using StudHunter.API.ModelsDto.MessageDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing messages.
/// </summary>
public class MessageService(StudHunterDbContext context) : BaseMessageService(context)
{
    public async Task<(MessageDto? Entity, int? StatusCode, string? ErrorMessage)> CreateInvitationMessageAsync(Guid senderId, Guid chatId, Guid invitationId, Guid? vacancyId, Guid? resumeId)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        if (chat == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Chat)));

        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("sender"));

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = $"{nameof(Invitation)}: {(vacancyId != null ? nameof(Vacancy) : nameof(Resume))}Id = {vacancyId ?? resumeId}",
            InvitationId = invitationId,
            SentAt = DateTime.UtcNow
        };

        chat.LastMessageAt = DateTime.UtcNow;
        _context.Messages.Add(message);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Message>();
        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToMessageDto(message), statusCode, errorMessage);
    }
}
