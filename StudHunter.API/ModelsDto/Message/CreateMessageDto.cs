using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Message;

public class CreateMessageDto
{
    [Required(ErrorMessage = "{0} is required")]
    public Guid ReceiverId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Context { get; set; } = string.Empty;
}
