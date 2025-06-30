using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

public class UpdateEmployerDto
{
    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    [MinLength(8)]
    public string? Password { get; set; }

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Url, MaxLength(255)]
    public string? Website { get; set; }

    [MaxLength(255)]
    public string? Specialization { get; set; }
}
    