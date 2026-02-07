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
    string? Specialization,
    string? Inn,
    string? Ogrn,
    string? LegalAddress,
    string? ActualAddress,
    DateTime CreatedAt,
    int VacanciesCount);

public record AdminEmployerDto(
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
    string? Specialization,
    string? Inn,
    string? Ogrn,
    string? LegalAddress,
    string? ActualAddress,
    DateTime CreatedAt,
    int VacanciesCount,
    bool IsDeleted)
    : EmployerDto(Id, Email, RegistrationStage, Name, CityId, CityName, ContactPhone, ContactEmail,
        AvatarUrl, Description, Website, Specialization, Inn, Ogrn, LegalAddress, ActualAddress, CreatedAt, VacanciesCount);

public record UpdateEmployerDto(
    [StringLength(255)] string? Name,
    Guid? CityId,
    [StringLength(1000)] string? Description,
    [Phone] string? ContactPhone,
    [EmailAddress] string? ContactEmail,
    [StringLength(255)] string? AvatarUrl,
    [Url] string? Website,
    [StringLength(255)] string? Specialization,
    [StringLength(12)] string? Inn,
    [StringLength(15)] string? Ogrn,
    string? LegalAddress,
    string? ActualAddress,
    [StringLength(255, MinimumLength = 8)] string? Password);

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
        e.Specialization,
        e.OrganizationDetails?.Inn,
        e.OrganizationDetails?.Ogrn,
        e.OrganizationDetails?.LegalAddress,
        e.OrganizationDetails?.ActualAddress,
        e.CreatedAt,
        e.Vacancies?.Count(v => !v.IsDeleted) ?? 0);

    public static AdminEmployerDto ToAdminDto(Employer e)
    {
        var vacanciesCount = e.Vacancies?.Count(v => !v.IsDeleted) ?? 0;

        return new AdminEmployerDto(
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
            e.Specialization,
            e.OrganizationDetails?.Inn,
            e.OrganizationDetails?.Ogrn,
            e.OrganizationDetails?.LegalAddress,
            e.OrganizationDetails?.ActualAddress,
            e.CreatedAt,
            vacanciesCount,
            e.IsDeleted);
    }

    public static void ApplyUpdate(Employer employer, UpdateEmployerDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name))
            employer.Name = dto.Name.Trim();

        if (dto.CityId.HasValue) employer.CityId = dto.CityId.Value;
        if (dto.Description != null) employer.Description = dto.Description.Trim();
        if (dto.ContactPhone != null) employer.ContactPhone = dto.ContactPhone.Trim();
        if (dto.ContactEmail != null) employer.ContactEmail = dto.ContactEmail.Trim();
        if (dto.AvatarUrl != null) employer.AvatarUrl = dto.AvatarUrl;
        if (dto.Website != null) employer.Website = dto.Website.Trim();
        if (dto.Specialization != null) employer.Specialization = dto.Specialization.Trim();

        if (dto.Inn != null || dto.Ogrn != null || dto.LegalAddress != null || dto.ActualAddress != null)
        {
            if (employer.OrganizationDetails == null)
            {
                employer.OrganizationDetails = new OrganizationDetail { EmployerId = employer.Id };
            }

            if (dto.Inn != null) employer.OrganizationDetails.Inn = dto.Inn.Trim();
            if (dto.Ogrn != null) employer.OrganizationDetails.Ogrn = dto.Ogrn.Trim();
            if (dto.LegalAddress != null) employer.OrganizationDetails.LegalAddress = dto.LegalAddress.Trim();
            if (dto.ActualAddress != null) employer.OrganizationDetails.ActualAddress = dto.ActualAddress.Trim();
        }
    }
}