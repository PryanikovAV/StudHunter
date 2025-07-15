using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class AdministratorMessageController(AdministratorMessagesService administratorMessagesService) : ControllerBase
{
    private readonly AdministratorMessagesService _administratorMessagesService = administratorMessagesService;

    [HttpGet("messages")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAllMessages()
    {
        var messages = await _administratorMessagesService.GetAllMessageAsync();
        return Ok(messages);
    }

    [HttpGet("user/{userId}/sent")]
    public async Task<IActionResult> GetSentMessages(Guid userId)
    {
        var messages = await _administratorMessagesService.GetMessagesByUserAsync(userId, sent: true);
        return Ok(messages);
    }

    [HttpGet("user/{userId}/received")]
    public async Task<IActionResult> GetReceivedMessages(Guid userId)
    {
        var messages = await _administratorMessagesService.GetMessagesByUserAsync(userId, sent: false);
        return Ok(messages);
    }

    [HttpDelete("message/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteMessage(Guid id)
    {
        var (success, error) = await _administratorMessagesService.DeleteMessageAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
