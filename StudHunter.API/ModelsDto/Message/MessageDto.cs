using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Message;

public class MessageDto
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public string SenderEmail { get; set; } = string.Empty;

    public Guid ReceiverId { get; set; }

    public string ReceiverEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Context { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
}
