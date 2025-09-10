using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.ChatDto;
using StudHunter.API.Services;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController(ChatService chatService) : BaseController
{
    private readonly ChatService _chatService = chatService;

    /// <summary>
    /// Retrieves all chats for the authenticated user.
    /// </summary>
    /// <returns>A list of chats.</returns>
    /// <response code="200">Chats retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ChatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetChats()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<ChatDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (chats, statusCode, errorMessage) = await _chatService.GetChatsByUserAsync(authUserId);
        return HandleResponse(chats, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a chat by its ID for the authenticated user.
    /// </summary>
    /// <param name="chatId">The unique identifier (GUID) of the chat.</param>
    /// <returns>The chat.</returns>
    /// <response code="200">Chat retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Chat not found.</response>
    [HttpGet("{chatId}")]
    [ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChat(Guid chatId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<ChatDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (chats, statusCode, errorMessage) = await _chatService.GetChatByIdAsync(authUserId, chatId);
        return HandleResponse(chats, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new chat for the authenticated user.
    /// </summary>
    /// <returns>The created chat.</returns>
    /// <response code="201">Chat created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Receiver not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ChatDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<ChatDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<ChatDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (chat, statusCode, errorMessage) = await _chatService.CreateChatAsync(authUserId, dto.ReceiverId);
        return HandleResponse(chat, statusCode, errorMessage, nameof(GetChat), new { chatId = chat?.Id });
    }
}
