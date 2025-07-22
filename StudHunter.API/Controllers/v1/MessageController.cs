using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class MessageController(MessageService messageService) : ControllerBase
{
    private readonly MessageService _messageService = messageService;

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentMessages(Guid userId)
    {
        var messages = await _messageService.GetMessagesByUserAsync(userId, sent: true);
        return Ok(messages);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedMessages(Guid userId)
    {
        var messages = await _messageService.GetMessagesByUserAsync(userId, sent: false);
        return Ok(messages);
    }
    // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
    [HttpPost]
    public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var senderId = Guid.NewGuid();
        var (message, error) = await _messageService.CreateMessageAsync(senderId, dto);
        if (message == null)
            return error == null ? NotFound() : BadRequest(new { error });
        return CreatedAtAction(nameof(GetSentMessages), new { userId = message.SenderId }, message);
    }
}