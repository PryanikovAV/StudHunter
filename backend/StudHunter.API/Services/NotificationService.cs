using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface INotificationService
{
    Task SendAsync(Guid userId, string title, string message,
        Notification.NotificationType type, Guid? entityId = null, Guid? senderId = null);
    Task<Result<PagedResult<NotificationDto>>> GetMyNotificationsAsync(Guid userId, PaginationParams paging);
    Task<Result<bool>> MarkAsReadAsync(Guid userId, Guid notificationId);
    Task<Result<bool>> MarkAllAsReadAsync(Guid userId);
    Task<Result<bool>> MarkMultipleAsReadAsync(Guid userId, List<Guid> notificationIds);
}

public class NotificationService(StudHunterDbContext context,
    IHubContext<NotificationHub> hubContext,
    IRegistrationManager registrationManager)
    : BaseNotificationService(context, registrationManager), INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;

    public async Task SendAsync(Guid userId, string title, string message,
    Notification.NotificationType type, Guid? entityId = null, Guid? senderId = null)
    {
        if (senderId.HasValue)
        {
            var canCommunicate = await EnsureCommunicationAllowedAsync(userId, senderId.Value);
            if (!canCommunicate.IsSuccess)
                return;
        }

        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            EntityId = entityId
        };

        _context.Notifications.Add(notification);

        var result = await SaveChangesAsync<Notification>();
        if (result.IsSuccess)
        {
            var dto = NotificationMapper.ToDto(notification);
            await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", dto);
        }
    }

    public async Task<Result<PagedResult<NotificationDto>>> GetMyNotificationsAsync(Guid userId, PaginationParams paging)
    {
        var pagedNotifications = await GetBaseNotificationQuery(userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToPagedResultAsync(paging);

        var dtos = pagedNotifications.Items.Select(NotificationMapper.ToDto).ToList();
        
        var pageResult = new PagedResult<NotificationDto>(
            dtos, pagedNotifications.TotalCount, pagedNotifications.PageNumber, pagedNotifications.PageSize);

        return Result<PagedResult<NotificationDto>>.Success(pageResult);
    }

    public async Task<Result<bool>> MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Notification)), StatusCodes.Status404NotFound);

        notification.IsRead = true;
        return await SaveChangesAsync<Notification>();
    }

    public async Task<Result<bool>> MarkAllAsReadAsync(Guid userId)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkMultipleAsReadAsync(Guid userId, List<Guid> notificationIds)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && notificationIds.Contains(n.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        return Result<bool>.Success(true);
    }
}
