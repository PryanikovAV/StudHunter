using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.ChatDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseChatService(StudHunterDbContext context) : BaseService(context)
{
    protected static ChatDto MapToChatDto(Chat chat)
    {
        return new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.User1Id,
            User1Email = chat.User1.IsDeleted ? "[Deleted Account]" : chat.User1.Email,
            User2Id = chat.User2Id,
            User2Email = chat.User2.IsDeleted ? "[Deleted Account]" : chat.User2.Email,
            CreatedAt = chat.CreatedAt,
            LastMessageAt = chat.LastMessageAt
        };
    }

    public async Task<(List<ChatDto>? Entities, int? StatusCode, string? ErrorMessage)> GetChatsByUserAsync(Guid authUserId)
    {
        var chats = await _context.Chats
            .Where(c => c.User1Id == authUserId || c.User2Id == authUserId)
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Select(c => MapToChatDto(c))
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();

        return (chats, null, null);
    }

    public async Task<(ChatDto? Entity, int? StatusCode, string? ErrorMessage)> GetChatByIdAsync(Guid authUserId, Guid chatId)
    {
        var chat = await _context.Chats
            .Where(c => c.Id == chatId && (c.User1Id == authUserId || c.User2Id == authUserId))
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync();

        if (chat == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Chat)));

        return (MapToChatDto(chat), null, null);
    }

    public async Task<(ChatDto? Entity, int? StatusCode, string? ErrorMessage)> CreateChatAsync(Guid authUserId, Guid receiverId)
    {
        if (authUserId == receiverId)
            return (null, StatusCodes.Status400BadRequest, $"Sender and receiver cannot be the same.");

        var sender = await _context.Users.FindAsync(authUserId);
        if (sender == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("sender"));

        var receiver = await _context.Users.FindAsync(receiverId);
        if (receiver == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("receiver"));

        if ((sender is Student && receiver is Student) || (sender is Employer && receiver is Employer))
            return (null, StatusCodes.Status400BadRequest, $"{nameof(Chat)} can only be created between {nameof(Student)} and {nameof(Employer)} or with an admin.");

        var user1Id = authUserId < receiverId ? authUserId : receiverId;
        var user2Id = authUserId < receiverId ? receiverId : authUserId;

        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.User1Id == user1Id && c.User2Id == user2Id);

        if (chat != null)
            return (MapToChatDto(chat), null, null);

        chat = new Chat
        {
            Id = Guid.NewGuid(),
            User1Id = user1Id,
            User2Id = user2Id,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = null
        };

        _context.Chats.Add(chat);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Chat>();
        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToChatDto(chat), null, null);
    }
}
