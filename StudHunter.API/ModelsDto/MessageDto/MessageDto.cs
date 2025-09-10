namespace StudHunter.API.ModelsDto.MessageDto;

/// <summary>
/// Data transfer object for a message.
/// </summary>
public class MessageDto
{
    /// <summary>
    /// The unique identifier (GUID) of the message.
    /// </summary>
    public Guid Id { get; set; }

    public Guid ChatId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the sender.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// The email address of the sender.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// The content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    public Guid? InvitationId { get; set; }

    /// <summary>
    /// The date and time the message was sent.
    /// </summary>
    public DateTime SentAt { get; set; }
}
