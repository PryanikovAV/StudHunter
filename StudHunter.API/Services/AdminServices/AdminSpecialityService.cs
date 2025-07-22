using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminSpecialityService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<SpecialityDto?> GetSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);

        if (speciality == null)
            return null;

        return new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        };
    }

    public async Task<(SpecialityDto? Speciality, string? Error)> CreateSpecialityAsync(CreateSpecialityDto dto)
    {
        if (await _context.Specialities.AnyAsync(s => s.Name == dto.Name))
            return (null, "Speciality with this name already exists.");

        var speciality = new Speciality
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Specialities.Add(speciality);

        var (success, error) = await SaveChangesAsync("create", "Speciality");
        if (!success)
            return (null, error);

        return (new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        }, null);
    }

    public async Task<(bool Success, string? Error)> UpdateSpecialityAsync(Guid id, UpdateSpecialityDto dto)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);

        if (speciality == null)
            return (false, "Speciality not found.");

        if (await _context.Specialities.AnyAsync(s => s.Name == dto.Name && s.Id != id))
            return (false, "Speciality with this name already exists.");

        if (dto.Name != null)
            speciality.Name = dto.Name;
        if (dto.Description != null)
            speciality.Description = dto.Description;

        return await SaveChangesAsync("update", "Speciality");
    }

    public async Task<(bool Success, string? Error)> DeleteSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);

        if (speciality == null)
            return (false, "Speciality not found");

        if (await _context.StudyPlans.AnyAsync(sp => sp.SpecialityId == id))
            return (false, "Cannot delete speciality associated with study plans");

        return await HardDeleteEntityAsync<Speciality>(id);
    }
}
