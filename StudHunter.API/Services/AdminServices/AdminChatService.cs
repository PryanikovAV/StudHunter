using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.ChatDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminChatService(StudHunterDbContext context) : BaseChatService(context)
{
    public async Task<(List<ChatDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllChatsAsync()
    {
        var chats = await _context.Chats
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Select(c => MapToChatDto(c))
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();

        return (chats, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteChatAsync(Guid chatId)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        if (chat == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Chat)));

        _context.Chats.Remove(chat);
        return await SaveChangesAsync<Chat>();
    }
}
