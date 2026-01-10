using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record EmployerDto(
    Guid Id,
    string Email,
    string RegistrationStage,
    string Name,
    string? ContactPhone,
    string? ContactEmail,
    string? AvatarUrl,
    string? Description,
    string? Website,
    string? Specialization,
    DateTime CreatedAt,
    int VacanciesCount);

public record AdminEmployerDto(
    Guid Id,
    string Email,
    string RegistrationStage,
    string Name,
    string? ContactPhone,
    string? ContactEmail,
    string? AvatarUrl,
    string? Description,
    string? Website,
    string? Specialization,
    DateTime CreatedAt,
    int VacanciesCount,
    bool IsDeleted)
    : EmployerDto(Id, Email, RegistrationStage, Name, ContactPhone, ContactEmail, AvatarUrl,
        Description, Website, Specialization, CreatedAt, VacanciesCount);

public record UpdateEmployerDto(
    [StringLength(255)] string? Name,
    [StringLength(1000)] string? Description,
    [Phone] string? ContactPhone,
    [EmailAddress] string? ContactEmail,
    [StringLength(255)] string? AvatarUrl,
    [Url] string? Website,
    [StringLength(255)] string? Specialization,
    [StringLength(255, MinimumLength = 8)] string? Password);

public static class EmployerMapper
{
    public static EmployerDto ToDto(Employer e) => new(
        e.Id,
        e.Email,
        e.RegistrationStage.ToString(),
        e.Name,
        e.ContactPhone,
        e.ContactEmail,
        e.AvatarUrl,
        e.Description,
        e.Website,
        e.Specialization,
        e.CreatedAt,
        e.Vacancies?.Count(v => !v.IsDeleted) ?? 0);

    public static AdminEmployerDto ToAdminDto(Employer e, int count) => new(
        e.Id,
        e.Email,
        e.RegistrationStage.ToString(),
        e.Name,
        e.ContactPhone,
        e.ContactEmail,
        e.AvatarUrl,
        e.Description,
        e.Website,
        e.Specialization,
        e.CreatedAt,
        count,
        e.IsDeleted);

    public static AdminEmployerDto ToAdminDto(Employer e) =>
        ToAdminDto(e, e.Vacancies?.Count(v => !v.IsDeleted) ?? 0);

    public static void ApplyUpdate(Employer employer, UpdateEmployerDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name))
            employer.Name = dto.Name.Trim();

        if (dto.Description != null) employer.Description = dto.Description.Trim();
        if (dto.ContactPhone != null) employer.ContactPhone = dto.ContactPhone.Trim();
        if (dto.ContactEmail != null) employer.ContactEmail = dto.ContactEmail.Trim();
        if (dto.AvatarUrl != null) employer.AvatarUrl = dto.AvatarUrl;
        if (dto.Website != null) employer.Website = dto.Website.Trim();
        if (dto.Specialization != null) employer.Specialization = dto.Specialization.Trim();
    }
}