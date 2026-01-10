using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1/notifications")]
public class NotificationController(INotificationService notificationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] PaginationParams paging) =>
        HandleResult(await notificationService.GetMyNotificationsAsync(AuthorizedUserId, paging));

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id) =>
        HandleResult(await notificationService.MarkAsReadAsync(AuthorizedUserId, id));

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead() =>
        HandleResult(await notificationService.MarkAllAsReadAsync(AuthorizedUserId));

    [HttpPatch("read-multiple")]
    public async Task<IActionResult> MarkMultipleAsRead([FromBody] MarkAsReadRequest request) =>
        HandleResult(await notificationService.MarkMultipleAsReadAsync(AuthorizedUserId, request.NotificationIds));
}
