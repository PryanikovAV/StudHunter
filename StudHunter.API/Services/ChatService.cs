using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing chats.
/// </summary>
public class ChatService(StudHunterDbContext context) : BaseChatService(context)
{

}
