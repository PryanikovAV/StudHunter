using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1/chats")]
public class ChatController(IChatService chatService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyChats([FromQuery] PaginationParams paging) =>
        HandleResult(await chatService.GetMyChatsAsync(AuthorizedUserId, paging));

    [HttpGet("{chatId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid chatId, [FromQuery] PaginationParams paging) =>
        HandleResult(await chatService.GetChatMessagesAsync(AuthorizedUserId, chatId, paging));

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request) =>
        HandleResult(await chatService.SendMessageAsync(
            AuthorizedUserId,
            request.ReceiverId,
            request.Content,
            request.InvitationId));

    [HttpGet("interlocutor/{userId:guid}")]
    public async Task<IActionResult> GetChatParticipant(Guid userId) =>
        HandleResult(await chatService.GetChatParticipantAsync(userId));

    [HttpPatch("{chatId:guid}/read")]
    public async Task<IActionResult> MarkChatAsRead(Guid chatId) =>
        HandleResult(await chatService.MarkMessagesAsReadAsync(AuthorizedUserId, chatId));
}