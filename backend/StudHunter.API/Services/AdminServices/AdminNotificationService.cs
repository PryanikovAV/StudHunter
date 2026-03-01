using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminNotificationService
{
    Task<Result<bool>> SendMassNotificationAsync(SendMassNotificationRequest request);
}

public class AdminNotificationService(
    StudHunterDbContext context,
    IRegistrationManager registrationManager,
    IHubContext<NotificationHub> hubContext)
    : BaseService(context, registrationManager), IAdminNotificationService
{
    public async Task<Result<bool>> SendMassNotificationAsync(SendMassNotificationRequest request)
    {
        var query = _context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.TargetRole))
        {
            if (request.TargetRole == UserRoles.Student) query = query.OfType<Student>();
            else if (request.TargetRole == UserRoles.Employer) query = query.OfType<Employer>();
        }

        var userIds = await query.Select(u => u.Id).ToListAsync();
        if (!userIds.Any()) return Result<bool>.Success(true);

        var notifications = userIds.Select(userId => new Notification
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            Type = Notification.NotificationType.System,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _context.Notifications.AddRangeAsync(notifications);
        var dbResult = await SaveChangesAsync<Notification>();

        if (!dbResult.IsSuccess) return Result<bool>.Failure(dbResult.ErrorMessage!);

        var dto = NotificationMapper.ToDto(notifications.First());
        await hubContext.Clients.All.SendAsync("ReceiveNotification", dto);

        return Result<bool>.Success(true);
    }
}