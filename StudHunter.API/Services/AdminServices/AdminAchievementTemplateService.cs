using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing achievement templates for administrators.
/// </summary>
public class AdminAchievementTemplateService(StudHunterDbContext context) : BaseAchievementTemplateService(context)
{
    /// <summary>
    /// Retrieves all achievement templates.
    /// </summary>
    /// <returns>A tuple containing a list of all achievement templates, an optional status code, and an optional error message.</returns>
    public async Task<(List<AchievementTemplateDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllAchievementTemplatesAsync()
    {
        var templates = await _context.AchievementTemplates
        .Select(a => MapToAchievementTemplateDto(a))
        .ToListAsync();

        return (templates, null, null);
    }

    /// <summary>
    /// Retrieves an achievement template by its id.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>A tuple containing the achievement template, an optional status code, and an optional error message.</returns>
    public async Task<(AchievementTemplateDto? Entity, int? StatusCode, string? ErrorMessage)> GetAchievementTemplateAsync(Guid id)
    {
        var template = await _context.AchievementTemplates.FindAsync(id);
        if (template == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(AchievementTemplate)));

        return (MapToAchievementTemplateDto(template), null, null);
    }

    /// <summary>
    /// Creates a new achievement template.
    /// </summary>
    /// <param name="dto">The data transfer object containing achievement template details.</param>
    /// <returns>A tuple containing the created achievement template, an optional status code, and an optional error message.</returns>
    public async Task<(AchievementTemplateDto? Entity, int? StatusCode, string? ErrorMessage)> CreateAchievementTemplateAsync(CreateAchievementTemplateDto dto)
    {
        if (await _context.AchievementTemplates.AnyAsync(a => a.Name == dto.Name))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(AchievementTemplate), "Name"));

        var template = new AchievementTemplate
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Target = Enum.Parse<AchievementTemplate.AchievementTarget>(dto.Target),
            IconUrl = dto.IconUrl
        };

        _context.AchievementTemplates.Add(template);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<AchievementTemplate>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToAchievementTemplateDto(template), null, null);
    }

    /// <summary>
    /// Updates an existing achievement template.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the achievement template.</param>
    /// <param name="dto">The data transfer object containing updated achievement template details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateAchievementTemplateAsync(Guid id, UpdateAchievementTemplateDto dto)
    {
        var template = await _context.AchievementTemplates.FindAsync(id);

        if (template == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(AchievementTemplate)));

        if (dto.Name != null && await _context.AchievementTemplates.AnyAsync(a => a.Name == dto.Name && a.Id != id))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(AchievementTemplate), "name"));

        if (dto.Name != null)
            template.Name = dto.Name;
        if (dto.Description != null)
            template.Description = dto.Description;
        if (dto.Target != null)
            template.Target = Enum.Parse<AchievementTemplate.AchievementTarget>(dto.Target);
        if (dto.IconUrl != null)
            template.IconUrl = dto.IconUrl;

        return await SaveChangesAsync<AchievementTemplate>();
    }

    /// <summary>
    /// Deletes an achievement template.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteAchievementTemplateAsync(Guid id)
    {
        return await DeleteEntityAsync<AchievementTemplate>(id, hardDelete: true);
    }
}
