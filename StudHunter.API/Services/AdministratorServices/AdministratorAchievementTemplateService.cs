using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdministratorServices;

public class AdministratorAchievementTemplateService(StudHunterDbContext context) : BaseEntityService(context)
{
    public async Task<IEnumerable<AchievementTemplateDto>> GetAllAchievementTemplatesAsync()
    {
        return await _context.AchievementTemplates.Select(a => new AchievementTemplateDto
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            Target = a.Target.ToString()
        })
        .ToListAsync();
    }
    public async Task<AchievementTemplateDto?> GetAchievementTemplateAsync(int id)
    {
        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.Id == id);

        if (template == null)
            return null;

        return new AchievementTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            Target = template.Target.ToString()
        };
    }

    public async Task<(AchievementTemplateDto? Template, string? Error)> CreateAchievementTemplateAsync(CreateAchievementTemplateDto dto)
    {
        if (await _context.AchievementTemplates.AnyAsync(a => a.Name == dto.Name))
            return (null, "Achievement template with this name already exists");

        var template = new AchievementTemplate
        {
            Name = dto.Name,
            Description = dto.Description,
            Target = Enum.Parse<AchievementTemplate.AchievementTarget>(dto.Target)
        };

        _context.AchievementTemplates.Add(template);

        var (success, error) = await SaveChangesAsync("create", "AchievementTemplate");
        if (!success)
            return (null, error);

        return (new AchievementTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            Target = template.Target.ToString()
        }, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAchievementTemplateAsync(int id, UpdateAchievementTemplateDto dto)
    {
        var template = await _context.AchievementTemplates.FirstOrDefaultAsync(a => a.Id == id);

        if (template == null)
            return (false, "Achievement template not found");

        if (dto.Name != null && await _context.AchievementTemplates.AnyAsync(a => a.Name == dto.Name && a.Id != id))
            return (false, "Achievement template with this name already exists");

        if (dto.Name != null)
            template.Name = dto.Name;

        if (dto.Description != null)
            template.Description = dto.Description;

        if (dto.Target != null)
            template.Target = Enum.Parse<AchievementTemplate.AchievementTarget>(dto.Target);

        return await SaveChangesAsync("update", "AchievementTemplate");
    }

    public async Task<(bool Success, string? Error)> DeleteAchievementTemplateAsync(int id)
    {
        return await HardDeleteEntityAsync<AchievementTemplate>(id);
    }
}
