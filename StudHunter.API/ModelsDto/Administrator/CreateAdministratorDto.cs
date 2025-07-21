using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Administrator;

public class CreateAdministratorDto
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1}")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} is required")]
    public bool IsDeleted { get; set; } = false;

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? ContactEmail { get; set; }

    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string LastName { get; set; } = string.Empty;

    [Required, RegularExpression("SuperAdmin|Moderator", ErrorMessage = "{0} must be 'SuperAdmin' or 'Moderator'")]
    public string AdminLevel { get; set; } = string.Empty;
}
