using StudHunter.DB.Postgres.Interfaces;

namespace StudHunter.DB.Postgres.Models;

public class Message : IEntity
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public string Context { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public virtual User Sender { get; set; } = null!;
    public virtual User Receiver { get; set; } = null!;
}
