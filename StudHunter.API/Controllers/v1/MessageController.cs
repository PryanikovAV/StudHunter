using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services;

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
    /// Retrieves sent messages for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of sent messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("sent/{userId}")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSentMessages(Guid userId)
    {
        var (messages, statusCode, errorMessage) = await _messageService.GetMessagesByUserAsync(userId, sent: true);
        return CreateAPIError(messages, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves received messages for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of received messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("received/{userId}")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetReceivedMessages(Guid userId)
    {
        var (messages, statusCode, errorMessage) = await _messageService.GetMessagesByUserAsync(userId, sent: false);
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
        var userId = Guid.NewGuid(); // TODO: Replace Guid.NewGuid() with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
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
            return BadRequest(new { error = "Invalid request data." });

        var senderId = Guid.NewGuid(); // TODO: Replace Guid.NewGuid() with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (message, statusCode, errorMessage) = await _messageService.CreateMessageAsync(senderId, dto);
        return CreateAPIError(message, statusCode, errorMessage, nameof(GetMessage), new { id = message?.Id });
    }
}
