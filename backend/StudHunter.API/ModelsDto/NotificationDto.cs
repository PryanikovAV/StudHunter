using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto;

public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt,
    string Type,
    Guid? EntityId,
    string TimeAgo
);

public record MarkAsReadRequest(List<Guid> NotificationIds);

public static class NotificationMapper
{
    public static NotificationDto ToDto(Notification notification)
    {
        return new NotificationDto(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.IsRead,
            notification.CreatedAt,
            notification.Type.ToString(),
            notification.EntityId,
            GetRelativeTime(notification.CreatedAt)
        );
    }

    private static string GetRelativeTime(DateTime dateTime)
    {
        var span = DateTime.UtcNow - dateTime;
        if (span.TotalMinutes < 1) return "только что";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} мин. назад";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours} час. назад";
        return dateTime.ToString("dd.MM.yyyy HH:mm");
    }
}