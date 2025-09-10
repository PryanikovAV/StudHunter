using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.InvitationDto;

/// <summary>
/// Data transfer object for an invitation.
/// </summary>
public class InvitationDto
{
    /// <summary>
    /// The unique identifier (GUID) of the invitation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the sender.
    /// </summary>
    public Guid SenderId { get; set; }

    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier (GUID) of the receiver.
    /// </summary>
    public Guid ReceiverId { get; set; }

    public string ReceiverEmail { get; set; } = string.Empty;

    public EntitySummaryDto? Entity { get; set; }

    /// <summary>
    /// The status of the invitation (Sent, Accepted, or Rejected).
    /// </summary>
    [RegularExpression("Sent|Accepted|Rejected")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The date and time the invitation was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time the invitation was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

public class EntitySummaryDto
{
    public Guid? Id { get; set; }
    public string? Type { get; set; }
    public string? Title { get; set; }
    public string? Status { get; set; }
}
