namespace StudHunter.API.ModelsDto.ChatDto;

public class ChatDto
{
    public Guid Id { get; set; }

    public Guid User1Id { get; set; }

    public string User1Email { get; set; } = string.Empty;

    public Guid User2Id { get; set; }

    public string User2Email { get; set;} = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastMessageAt { get; set; }
}
