using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/chats")]
public class AdminChatController(IAdminChatService adminChatService) : BaseController
{
    [HttpDelete("messages/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId) =>
        HandleResult(await adminChatService.DeleteMessageAsync(messageId));

    [HttpGet("{chatId:guid}/inspect")]
    public async Task<IActionResult> InspectChat(Guid chatId, [FromQuery] PaginationParams paging, [FromQuery] Guid userId) =>
        HandleResult(await adminChatService.GetChatMessagesAsync(userId, chatId, paging));
}