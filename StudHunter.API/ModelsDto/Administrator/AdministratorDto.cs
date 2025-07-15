using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Administrator;

public class AdministratorDto
{
    public Guid Id { get; set; }

    public string Role => "Administrator";

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, RegularExpression("SuperAdmin|Moderator",
        ErrorMessage = "AdminLevel must be 'SuperAdmin' or 'Moderator'")]
    public string AdminLevel { get; set; } = string.Empty;
}