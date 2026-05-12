using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record EmployerDto(
    Guid Id,
    string Email,
    string RegistrationStage,
    string Name,
    Guid? CityId,
    string? CityName,
    string? ContactPhone,
    string? ContactEmail,
    string? AvatarUrl,
    string? Description,
    string? Website,
    Guid? SpecializationId,
    string? SpecializationName,
    string? Inn,
    string? Ogrn,
    string? Kpp,
    string? LegalAddress,
    string? ActualAddress,
    DateTime CreatedAt,
    int ActiveVacanciesCount
);

public record EmployerHeroDto(
    Guid Id,
    string Name,
    string? AvatarUrl,
    string? CityName,
    string? SpecializationName,
    string? Website,
    string? ContactEmail,
    string? ContactPhone,
    string RegistrationStage,
    string? Inn,
    string? Ogrn,
    string? Kpp,
    string? LegalAddress,
    string? ActualAddress,
    int ActiveVacanciesCount,
    bool IsFavorite = false,
    bool IsBlocked = false
);

public record AdminEmployerDto(
    Guid Id,
    string Email,
    string RegistrationStage,
    string Name, Guid? CityId,
    string? CityName,
    string? ContactPhone,
    string? ContactEmail,
    string? AvatarUrl,
    string? Description,
    string? Website,
    Guid? SpecializationId,
    string? SpecializationName,
    string? Inn,
    string? Ogrn,
    string? Kpp,
    string? LegalAddress,
    string? ActualAddress,
    DateTime CreatedAt,
    int ActiveVacanciesCount,
    bool IsDeleted)
    : EmployerDto(Id, Email, RegistrationStage, Name, CityId, CityName, ContactPhone, ContactEmail,
        AvatarUrl, Description, Website, SpecializationId, SpecializationName, Inn, Ogrn, Kpp, LegalAddress, ActualAddress, CreatedAt, ActiveVacanciesCount);

public record UpdateEmployerDto(
    [Required][StringLength(255)] string Name,
    Guid? CityId,
    [StringLength(1000)] string? Description,
    [Phone] string? ContactPhone,
    [EmailAddress] string? ContactEmail,
    [Url] string? Website,
    Guid? SpecializationId,
    [StringLength(12)] string? Inn,
    [StringLength(15)] string? Ogrn,
    [StringLength(9)] string? Kpp,
    string? LegalAddress,
    string? ActualAddress
);

public static class EmployerMapper
{
    public static EmployerDto ToDto(Employer e) => new(
        e.Id,
        e.Email,
        e.RegistrationStage.ToString(),
        e.Name, 
        e.CityId,
        e.City?.Name,
        e.ContactPhone,
        e.ContactEmail,
        e.AvatarUrl,
        e.Description,
        e.Website, 
        e.SpecializationId,
        e.Specialization?.Name,
        e.OrganizationDetails?.Inn,
        e.OrganizationDetails?.Ogrn,
        e.OrganizationDetails?.Kpp,
        e.OrganizationDetails?.LegalAddress, 
        e.OrganizationDetails?.ActualAddress,
        e.CreatedAt,
        e.Vacancies?.Count(v => !v.IsDeleted) ?? 0);

    public static EmployerHeroDto ToHeroDto(Employer employer, bool isFavorite = false, bool isBlocked = false) => new(
        Id: employer.Id,
        Name: employer.Name,
        AvatarUrl: employer.AvatarUrl,
        CityName: employer.City?.Name,
        SpecializationName: employer.Specialization?.Name,
        Website: employer.Website,
        ContactEmail: employer.ContactPhone,
        ContactPhone: employer.ContactEmail,
        RegistrationStage: employer.RegistrationStage.ToString(),

        Inn: employer.OrganizationDetails?.Inn,
        Ogrn: employer.OrganizationDetails?.Ogrn,
        Kpp: employer.OrganizationDetails?.Kpp,
        LegalAddress: employer.OrganizationDetails?.LegalAddress,
        ActualAddress: employer.OrganizationDetails?.ActualAddress,

        ActiveVacanciesCount: employer.Vacancies?.Count(v => !v.IsDeleted) ?? 0,
        IsFavorite: isFavorite,
        IsBlocked: isBlocked);

    public static AdminEmployerDto ToAdminDto(Employer e) => new(
        e.Id, 
        e.Email,
        e.RegistrationStage.ToString(),
        e.Name,
        e.CityId, 
        e.City?.Name, 
        e.ContactPhone, 
        e.ContactEmail,
        e.AvatarUrl, 
        e.Description, 
        e.Website,
        e.SpecializationId,
        e.Specialization?.Name,
        e.OrganizationDetails?.Inn,
        e.OrganizationDetails?.Ogrn,
        e.OrganizationDetails?.Kpp,
        e.OrganizationDetails?.LegalAddress,
        e.OrganizationDetails?.ActualAddress,
        e.CreatedAt, 
        e.Vacancies?.Count(v => !v.IsDeleted) ?? 0, 
        e.IsDeleted);

    public static void ApplyUpdate(Employer employer, UpdateEmployerDto dto)
    {
        employer.Name = dto.Name.Trim();
        employer.CityId = dto.CityId;
        employer.Description = dto.Description?.Trim();
        employer.ContactPhone = dto.ContactPhone?.Trim();
        employer.ContactEmail = dto.ContactEmail?.Trim();
        employer.Website = dto.Website?.Trim();
        employer.SpecializationId = dto.SpecializationId;

        bool hasOrgData = !string.IsNullOrWhiteSpace(dto.Inn) ||
                          !string.IsNullOrWhiteSpace(dto.Ogrn) ||
                          !string.IsNullOrWhiteSpace(dto.Kpp) ||
                          !string.IsNullOrWhiteSpace(dto.LegalAddress) ||
                          !string.IsNullOrWhiteSpace(dto.ActualAddress);

        if (hasOrgData)
        {
            employer.OrganizationDetails ??= new OrganizationDetail { EmployerId = employer.Id };

            if (dto.Inn != null) employer.OrganizationDetails.Inn = dto.Inn.Trim();
            if (dto.Ogrn != null) employer.OrganizationDetails.Ogrn = dto.Ogrn.Trim();
            if (dto.Kpp != null) employer.OrganizationDetails.Kpp = dto.Kpp.Trim();
            if (dto.LegalAddress != null) employer.OrganizationDetails.LegalAddress = dto.LegalAddress.Trim();
            if (dto.ActualAddress != null) employer.OrganizationDetails.ActualAddress = dto.ActualAddress.Trim();
        }
    }
}