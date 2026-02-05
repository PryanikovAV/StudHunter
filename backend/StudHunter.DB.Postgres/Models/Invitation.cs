namespace StudHunter.DB.Postgres.Models;

public class Invitation
{
    public Guid Id { get; init; }

    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }

    public Guid? VacancyId { get; set; }
    public Guid? ResumeId { get; set; }

    public InvitationStatus Status { get; set; } = InvitationStatus.Sent;
    public InvitationType Type { get; set; }
    public string? Message { get; set; }  // TODO: нужно ли сохранять в архив сопроводительные сообщения, или перенести в чат?

    public string? SnapshotVacancyTitle { get; set; }
    public string? SnapshotSenderName { get; set; }
    public string? SnapshotReceiverName { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddDays(7);

    public virtual User Sender { get; set; } = null!;
    public virtual User Receiver { get; set; } = null!;
    public virtual Vacancy? Vacancy { get; set; }
    public virtual Resume? Resume { get; set; }

    public enum InvitationStatus
    {
        Sent = 0,       // Отправлено (ожидание)
        Accepted = 1,   // Принято
        Rejected = 2,   // Отклонено вручную
        Expired = 3,    // Просрочено (автоматически)
        Cancelled = 4   // Отозвано отправителем
    }

    public enum InvitationType
    {
        Response = 0,   // Отклик (Студент -> Работодатель)
        Offer = 1       // Предложение (Работодатель -> Студент)
    }
}
