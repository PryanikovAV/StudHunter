using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record ChangeAvatarDto(
    [Required][StringLength(2000)] string AvatarUrl
);
