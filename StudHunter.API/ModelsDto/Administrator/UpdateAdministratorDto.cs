using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Administrator;

public class UpdateAdministratorDto
{
    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    [MinLength(8)]
    public string? Password { get; set; }

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? LastName { get; set; }

    [RegularExpression("SuperAdmin|Moderator",
        ErrorMessage = "AdminLevel must be 'SuperAdmin' or 'Moderator'")]
    public string? AdminLevel { get; set; }
}
