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
public class AdminAchievementTemplateService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Retrieves all achievement templates.
    /// </summary>
    /// <returns>A tuple containing a list of all achievement templates, an optional status code, and an optional error message.</returns>
    public async Task<(List<AchievementTemplateDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllAchievementTemplatesAsync()
    {
        var template = await _context.AchievementTemplates.Select(a => new AchievementTemplateDto
        {
            Id = a.Id,
            OrderNumber = a.OrderNumber,
            Name = a.Name,
            Description = a.Description,
            Target = a.Target.ToString(),
            IconUrl = a.IconUrl
        }).ToListAsync();

        return (template, null, null);
    }

    /// <summary>
    /// Retrieves an achievement template by its order number.
    /// </summary>
    /// <param name="orderNumber">The order number of the achievement template.</param>
    /// <returns>A tuple containing the achievement template, an optional status code, and an optional error message.</returns>
    public async Task<(AchievementTemplateDto? Entity, int? StatusCode, string? ErrorMessage)> GetAchievementTemplateAsync(int orderNumber)
    {
        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.OrderNumber == orderNumber);

        #region Serializers
        if (template == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("AchievementTemplate"));
        #endregion

        return (new AchievementTemplateDto
        {
            Id = template.Id,
            OrderNumber = template.OrderNumber,
            Name = template.Name,
            Description = template.Description,
            Target = template.Target.ToString(),
            IconUrl = template.IconUrl
        }, null, null);
    }

    /// <summary>
    /// Creates a new achievement template.
    /// </summary>
    /// <param name="dto">The data transfer object containing achievement template details.</param>
    /// <returns>A tuple containing the created achievement template, an optional status code, and an optional error message.</returns>
    public async Task<(AchievementTemplateDto? Entity, int? StatusCode, string? ErrorMessage)> CreateAchievementTemplateAsync(CreateAchievementTemplateDto dto)
    {
        #region Serializers
        var nameExist = await _context.AchievementTemplates.AnyAsync(a => a.Name == dto.Name);
        if (nameExist)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Achievement", "Name"));

        var orderNumberExists = await _context.AchievementTemplates.AnyAsync(a => a.OrderNumber == dto.OrderNumber);
        if (orderNumberExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("AchievementTemplate", "OrderNumber"));
        #endregion

        var template = new AchievementTemplate
        {
            Id = Guid.NewGuid(),
            OrderNumber = dto.OrderNumber,
            Name = dto.Name,
            Description = dto.Description,
            Target = Enum.Parse<AchievementTemplate.AchievementTarget>(dto.Target),
            IconUrl = dto.IconUrl
        };

        _context.AchievementTemplates.Add(template);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<AchievementTemplate>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new AchievementTemplateDto
        {
            Id = template.Id,
            OrderNumber = template.OrderNumber,
            Name = template.Name,
            Description = template.Description,
            Target = template.Target.ToString(),
            IconUrl = template.IconUrl
        }, null, null);
    }

    /// <summary>
    /// Updates an existing achievement template.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the achievement template.</param>
    /// <param name="dto">The data transfer object containing updated achievement template details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateAchievementTemplateAsync(Guid id, UpdateAchievementTemplateDto dto)
    {
        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.Id == id);

        #region Serializers
        if (template == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("AchievementTemplate"));

        if (dto.Name != null)
        {
            var nameExists = await _context.AchievementTemplates.AnyAsync(a => a.Name == dto.Name && a.Id != id);
            if (nameExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("AchievementTemplate", "Name"));
        }

        if (dto.OrderNumber.HasValue)
        {
            var orderNumberExists = await _context.AchievementTemplates.AnyAsync(a => a.OrderNumber == dto.OrderNumber && a.Id != id);
            if (orderNumberExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("AchievementTemplate", "OrderNumber"));
        }
        #endregion

        if (dto.Name != null)
            template.Name = dto.Name;
        if (dto.OrderNumber.HasValue)
            template.OrderNumber = dto.OrderNumber.Value;
        if (dto.Description != null)
            template.Description = dto.Description;
        if (dto.Target != null)
            template.Target = Enum.Parse<AchievementTemplate.AchievementTarget>(dto.Target);
        if (dto.IconUrl != null)
            template.IconUrl = dto.IconUrl;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<AchievementTemplate>();

        return (success, statusCode, errorMessage);
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
