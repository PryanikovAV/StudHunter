using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class MessageController(MessageService messageService) : BaseController
{
    private readonly MessageService _messageService = messageService;

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentMessages(Guid userId)
    {
        var (messages, statusCode, errorMessage) = await _messageService.GetMessagesByUserAsync(userId, sent: true);
        return this.CreateAPIError(messages, statusCode, errorMessage);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedMessages(Guid userId)
    {
        var (messages, statusCode, errorMessage) = await _messageService.GetMessagesByUserAsync(userId, sent: false);
        return this.CreateAPIError(messages, statusCode, errorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessage(Guid id)
    {
        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (message, statusCode, errorMessage) = await _messageService.GetMessageAsync(userId, id);
        return this.CreateAPIError(message, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var senderId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (messages, statusCode, errorMessage) = await _messageService.CreateMessageAsync(senderId, dto);
        return this.CreateAPIError(messages, statusCode, errorMessage, nameof(GetMessage), new { id = messages?.Id });
    }
}