using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.ChatDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminChatController(AdminChatService adminChatService) : BaseController
{
    private readonly AdminChatService _adminChatService = adminChatService;

    /// <summary>
    /// Retrieves all chats for admin moderation.
    /// </summary>
    /// <returns>A list of all chats.</returns>
    /// <response code="200">Chats retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ChatDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllChats()
    {
        var (chats, statusCode, errorMessage) = await _adminChatService.GetAllChatsAsync();
        return HandleResponse(chats, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a chat for admin moderation.
    /// </summary>
    /// <param name="chatId">The unique identifier (GUID) of the chat.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Chat deleted successfully.</response>
    /// <response code="404">Chat not found.</response>
    [HttpDelete("{chatId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteChat(Guid chatId)
    {
        var (success, statusCode, errorMessage) = await _adminChatService.DeleteChatAsync(chatId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
