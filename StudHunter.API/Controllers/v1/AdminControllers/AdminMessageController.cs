using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminMessageController(AdminMessagesService adminMessagesService) : ControllerBase
{
    private readonly AdminMessagesService _adminMessagesService = adminMessagesService;

    [HttpGet("messages")]
    public async Task<IActionResult> GetAllMessages()
    {
        var messages = await _adminMessagesService.GetAllMessageAsync();
        return Ok(messages);
    }

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentMessages(Guid userId)
    {
        var messages = await _adminMessagesService.GetMessagesByUserAsync(userId, sent: true);
        return Ok(messages);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedMessages(Guid userId)
    {
        var messages = await _adminMessagesService.GetMessagesByUserAsync(userId, sent: false);
        return Ok(messages);
    }

    [HttpDelete("message/{id}")]
    public async Task<IActionResult> DeleteMessage(Guid id)
    {
        var (success, error) = await _adminMessagesService.DeleteMessageAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
