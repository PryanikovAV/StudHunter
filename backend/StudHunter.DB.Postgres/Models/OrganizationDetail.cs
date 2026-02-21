namespace StudHunter.DB.Postgres.Models;

public class OrganizationDetail
{
    public Guid Id { get; init; }
    public Guid EmployerId { get; set; }

    public string? Inn { get; set; }
    public string? Ogrn { get; set; }
    public string? LegalAddress { get; set; }
    public string? ActualAddress { get; set; }
    public string? Kpp { get; set; }

    public virtual Employer Employer { get; set; } = null!;
}