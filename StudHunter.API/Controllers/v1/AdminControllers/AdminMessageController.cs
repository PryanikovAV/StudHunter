using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.MessageDto;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing messages with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminMessageController(AdminMessageService adminMessageService) : BaseController
{
    private readonly AdminMessageService _adminMessageService = adminMessageService;

    /// <summary>
    /// Retrieves all messages for admin moderation.
    /// </summary>
    /// <returns>A list of all messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMessages()
    {
        var (messages, statusCode, errorMessage) = await _adminMessageService.GetAllMessagesAsync();
        return HandleResponse(messages, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves messages sent or received by a specific user for admin moderation.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="sent">If true, retrieves sent messages; if false, retrieves received messages.</param>
    /// <returns>A list of messages.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessagesByUser(Guid userId, [FromQuery] bool sent = false)
    {
        var (messages, statusCode, errorMessage) = await _adminMessageService.GetMessagesByUserAsync(userId, sent);
        return HandleResponse(messages, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a message for admin moderation.
    /// </summary>
    /// <param name="messageId">The unique identifier (GUID) of the message.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Message deleted successfully.</response>
    /// <response code="404">Message not found.</response>
    [HttpDelete("{messageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        var (success, statusCode, errorMessage) = await _adminMessageService.DeleteMessageAsync(messageId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
