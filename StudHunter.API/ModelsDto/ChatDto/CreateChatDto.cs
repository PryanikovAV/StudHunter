using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.ChatDto;

public class CreateChatDto
{
    [Required(ErrorMessage = "{0} is required")]
    public Guid ReceiverId { get; set; }
}
