using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

public class UpdateEmployerByAdministratorDto
{
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? ContactEmail { get; set; }

    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Name { get; set; }

    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }

    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Url(ErrorMessage = "Invalid {0} format")]
    public string? Website { get; set; }

    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Specialization { get; set; }

    public bool AccreditationStatus { get; set; }
}
