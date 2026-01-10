namespace StudHunter.DB.Postgres.Models;

public class Notification
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }  // <- notification target

    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public NotificationType Type { get; set; }

    public Guid? EntityId { get; set; }  // <- optional link to InvitationId, VacancyId or ChatMessageId

    public virtual User User { get; set; } = null!;

    public enum NotificationType
    {
        System = 0,             // Системное (тех. работы, новости)
        InvitationIncome = 1,   // Кто-то прислал новый отклик/оффер
        InvitationStatus = 2,   // Статус вашего отклика изменился
        ChatMessage = 3         // Новое сообщение в чате
    }
}