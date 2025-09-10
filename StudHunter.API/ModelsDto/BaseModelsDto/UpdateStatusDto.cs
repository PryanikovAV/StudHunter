namespace StudHunter.API.ModelsDto.BaseModelsDto;

public class UpdateStatusDto
{
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
}
