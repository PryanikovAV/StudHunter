using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Message;

/// <summary>
/// Data transfer object for a message.
/// </summary>
public class MessageDto
{
    /// <summary>
    /// The unique identifier (GUID) of the message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the sender.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// The email address of the sender.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier (GUID) of the receiver.
    /// </summary>
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// The email address of the receiver.
    /// </summary>
    public string ReceiverEmail { get; set; } = string.Empty;

    /// <summary>
    /// The content of the message.
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Context { get; set; } = string.Empty;

    /// <summary>
    /// The date and time the message was sent.
    /// </summary>
    public DateTime SentAt { get; set; }
}
