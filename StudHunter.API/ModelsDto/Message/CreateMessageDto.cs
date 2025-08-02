using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Message;

/// <summary>
/// Data transfer object for creating a message.
/// </summary>
public class CreateMessageDto
{
    /// <summary>
    /// The unique identifier (GUID) of the receiver.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// The content of the message.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Context { get; set; } = string.Empty;
}
