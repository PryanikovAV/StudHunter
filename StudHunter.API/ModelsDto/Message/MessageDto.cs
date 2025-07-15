using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Message;

public class MessageDto
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "Maximum length 1000 characters")]
    public string Context { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
}
