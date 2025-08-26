namespace StudHunter.DB.Postgres.Models;

public class Message
{
    public Guid Id { get; set; }

    public Guid ChatId { get; set; }

    public Guid SenderId { get; set; }

    public string Content { get; set; } = null!;

    public Guid? InvitationId { get; set; }

    public DateTime SentAt { get; set; }

    public virtual Chat Chat { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
    public virtual Invitation? Invitation { get; set; } = null!;
}
