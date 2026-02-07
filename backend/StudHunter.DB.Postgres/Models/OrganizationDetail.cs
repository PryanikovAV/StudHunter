namespace StudHunter.DB.Postgres.Models;

public class OrganizationDetail
{
    public Guid Id { get; init; }
    public Guid EmployerId { get; set; }

    public string Inn { get; set; } = null!;            // ИНН (10 или 12 цифр)
    public string Ogrn { get; set; } = null!;           // ОГРН
    public string LegalAddress { get; set; } = null!;   // Юридический адрес
    public string ActualAddress { get; set; } = null!;  // Фактический адрес
    public string? Kpp { get; set; }                    // КПП (опционально)

    public virtual Employer Employer { get; set; } = null!;
}