using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

public class UpdateStudentByAdministratorDto
{
    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? LastName { get; set; }

    [RegularExpression("Male|Female", ErrorMessage = "Gender must be 'Male' or 'Female'")]
    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Photo { get; set; }

    public bool? IsForeign { get; set; }

    public int? StatusId { get; set; }

    // ===== StudyPlan =====
    public int? CourseNumber { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? SpecialityId { get; set; }

    [RegularExpression("fulltime|parttime|correspondence", ErrorMessage = "StudyForm must be 'fulltime', 'parttime', or 'correspondence'")]
    public string? StudyForm { get; set; }

    public DateOnly? BeginYear { get; set; }
    // ===== StudyPlan =====
}
