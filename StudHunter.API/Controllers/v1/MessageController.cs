using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.MessageDto;
using StudHunter.API.Services;
using System.Security.Claims;

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

    /// <summary>
    /// Retrieves all messages in a chat for the authenticated user.
    /// </summary>
    /// <param name="chatId">The unique identifier (GUID) of the chat.</param>
    /// <returns>A list of messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Chat not found.</response>
    [HttpGet("chat/{chatId}")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessagesByChat(Guid chatId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<MessageDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (messages, statusCode, errorMessage) = await _messageService.GetMessagesByChatAsync(authUserId, chatId);
        return HandleResponse(messages, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a message by its ID for the authenticated user.
    /// </summary>
    /// <param name="messageId">The unique identifier (GUID) of the message.</param>
    /// <returns>The message.</returns>
    /// <response code="200">Message retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Message not found.</response>
    [HttpGet("{messageId}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessage(Guid messageId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<MessageDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (message, statusCode, errorMessage) = await _messageService.GetMessageByIdAsync(authUserId, messageId);
        return HandleResponse(message, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new message for the authenticated user.
    /// </summary>
    /// <param name="dto">The data transfer object containing message details.</param>
    /// <returns>The created message.</returns>
    /// <response code="201">Message created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Receiver not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<MessageDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<MessageDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (message, statusCode, errorMessage) = await _messageService.CreateMessageAsync(authUserId, dto);
        return HandleResponse(message, statusCode, errorMessage, nameof(GetMessage), new { messageId = message?.Id });
    }
}
