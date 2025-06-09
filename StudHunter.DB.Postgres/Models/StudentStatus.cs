using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class StudentStatus
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = null!;
}
