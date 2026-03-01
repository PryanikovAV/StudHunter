using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface INotificationService
{
    Task SendAsync(Guid userId, string title, string message, Notification.NotificationType type, Guid? entityId = null, Guid? senderId = null);
    Task<Result<PagedResult<NotificationDto>>> GetMyNotificationsAsync(Guid userId, PaginationParams paging);
    Task<Result<bool>> MarkAsReadAsync(Guid userId, Guid notificationId);
    Task<Result<bool>> MarkAllAsReadAsync(Guid userId);
    Task<Result<bool>> MarkMultipleAsReadAsync(Guid userId, List<Guid> notificationIds);
}

public class NotificationService(StudHunterDbContext context,
    IHubContext<NotificationHub> hubContext,
    IRegistrationManager registrationManager)
    : BaseService(context, registrationManager), INotificationService
{
    public async Task SendAsync(Guid userId, string title, string message, Notification.NotificationType type, Guid? entityId = null, Guid? senderId = null)
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

        if ((await SaveChangesAsync<Notification>()).IsSuccess)
        {
            var dto = NotificationMapper.ToDto(notification);
            await hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", dto);
        }
    }

    public async Task<Result<PagedResult<NotificationDto>>> GetMyNotificationsAsync(Guid userId, PaginationParams paging)
    {
        var pagedNotifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToPagedResultAsync(paging ?? new PaginationParams());

        var dtos = pagedNotifications.Items.Select(NotificationMapper.ToDto).ToList();

        return Result<PagedResult<NotificationDto>>.Success(new PagedResult<NotificationDto>(
            dtos, pagedNotifications.TotalCount, pagedNotifications.PageNumber, pagedNotifications.PageSize));
    }

    public async Task<Result<bool>> MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
        
        if (notification == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Notification)), StatusCodes.Status404NotFound);

        notification.IsRead = true;
        
        return await SaveChangesAsync<Notification>();
    }

    public async Task<Result<bool>> MarkAllAsReadAsync(Guid userId)
    {
        await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkMultipleAsReadAsync(Guid userId, List<Guid> notificationIds)
    {
        await _context.Notifications.Where(n => n.UserId == userId && notificationIds.Contains(n.Id)).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        return Result<bool>.Success(true);
    }
}