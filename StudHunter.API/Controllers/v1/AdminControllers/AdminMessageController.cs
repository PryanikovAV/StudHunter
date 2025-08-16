using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing messages with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminMessageController(AdminMessagesService adminMessagesService) : BaseController
{
    private readonly AdminMessagesService _adminMessagesService = adminMessagesService;

    /// <summary>
    /// Retrieves all messages.
    /// </summary>
    /// <returns>A list of all messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllMessages()
    {
        var (messages, statusCode, errorMessage) = await _adminMessagesService.GetAllMessagesAsync();
        return CreateAPIError(messages, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves sent messages for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of sent messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet("user/{userId}/sent")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSentMessages(Guid userId)
    {
        var (messages, statusCode, errorMessage) = await _adminMessagesService.GetMessagesByUserAsync(userId, sent: true);
        return CreateAPIError(messages, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves received messages for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of received messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet("user/{userId}/received")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReceivedMessages(Guid userId)
    {
        var (messages, statusCode, errorMessage) = await _adminMessagesService.GetMessagesByUserAsync(userId, sent: false);
        return CreateAPIError(messages, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessage(Guid id)
    {
        var (message, statusCode, errorMessage) = await _adminMessagesService.GetMessageAsync(id);
        return CreateAPIError(message, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a message.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the message.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Message deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Message not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMessage(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminMessagesService.DeleteMessageAsync(id);
        return CreateAPIError<MessageDto>(success, statusCode, errorMessage);
    }
}
