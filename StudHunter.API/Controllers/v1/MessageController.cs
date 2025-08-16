using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.ModelsDto.Chat;
using StudHunter.API.Services;
using System.Security.Claims;
using StudHunter.API.Common;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing messages.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class MessageController(MessageService messageService) : BaseController
{
    private readonly MessageService _messageService = messageService;

    [HttpGet("chats")]
    [ProducesResponseType(typeof(List<ChatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChatsByUser()
    {
        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<List<ChatDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (chats, statusCode, errorMessage) = await _messageService.GetChatsByUserAsync(userId);
        return CreateAPIError(chats, statusCode, errorMessage);
    }

    [HttpGet("chats/{chatId}")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessagesByChat(Guid chatId)
    {
        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<List<MessageDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (messages, statusCode, errorMessage) = await _messageService.GetMessagesByChatAsync(chatId, userId);
        return CreateAPIError(messages, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a message by its ID for the authenticated user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the message.</param>
    /// <returns>The message.</returns>
    /// <response code="200">Message retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Message not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessage(Guid id)
    {
        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<MessageDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (message, statusCode, errorMessage) = await _messageService.GetMessageAsync(id, userId);
        return CreateAPIError(message, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new message.
    /// </summary>
    /// <param name="dto">The data transfer object containing message details.</param>
    /// <returns>The created message.</returns>
    /// <response code="201">Message created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Sender or receiver not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var senderId))
            return CreateAPIError<MessageDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (message, statusCode, errorMessage) = await _messageService.CreateMessageAsync(senderId, dto);
        return CreateAPIError(message, statusCode, errorMessage, nameof(GetMessage), new { id = message?.Id });
    }
}
