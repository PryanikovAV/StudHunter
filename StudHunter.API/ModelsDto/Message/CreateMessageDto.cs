using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Message;

public class CreateMessageDto
{
    [Required]
    public Guid ReceiverId { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "Maximum length 1000 characters")]
    public string Context { get; set; } = string.Empty;
}
