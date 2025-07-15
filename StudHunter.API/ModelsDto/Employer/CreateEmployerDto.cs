using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

public class CreateEmployerDto
{
    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Url, MaxLength(255)]
    public string? Website { get; set; }

    [MaxLength(255)]
    public string? Specialization { get; set; }

    public bool AccreditationStatus = false;
}
