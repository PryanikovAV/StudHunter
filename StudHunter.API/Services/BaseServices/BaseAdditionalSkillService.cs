using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.AdditionalSkillDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseAdditionalSkillService(StudHunterDbContext context) : BaseService(context)
{
    protected static AdditionalSkillDto MapToAdditionalSkillDto(AdditionalSkill additionalSkillDto)
    {
        return new AdditionalSkillDto
        {
            Id = additionalSkillDto.Id,
            Name = additionalSkillDto.Name
        };
    }

    public async Task<(List<AdditionalSkillDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllAdditionalSkillsAsync()
    {
        var skills = await _context.AdditionalSkills
            .Select(s => MapToAdditionalSkillDto(s))
            .OrderByDescending(s => s.Name)
            .ToListAsync();

        return (skills, null, null);
    }

    public async Task<(AdditionalSkillDto? Entity, int? StatusCode, string? ErrorMessage)> GetAdditionalSkillAsync(Guid skillId)
    {
        var skill = await _context.AdditionalSkills.FirstOrDefaultAsync(s => s.Id == skillId);
        if (skill == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(AdditionalSkill)));
        return (MapToAdditionalSkillDto(skill), null, null);
    }

    public async Task<(AdditionalSkillDto? Entity, int? StatusCode, string? ErrorMessage)> GetAdditionalSkillAsync(string skillName)
    {
        var skill = await _context.AdditionalSkills.FirstOrDefaultAsync(s => s.Name == skillName);
        if (skill == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(AdditionalSkill)));
        return (MapToAdditionalSkillDto(skill), null, null);
    }
}
