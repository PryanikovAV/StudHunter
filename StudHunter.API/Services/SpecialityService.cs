using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class SpecialityService(StudHunterDbContext context)
{
    private readonly StudHunterDbContext _context = context;
    
    public async Task<IEnumerable<SpecialityDto>> GetSpecialitiesAsync()
    {
        return await _context.Specialities
            .Select(s => new SpecialityDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            })
            .ToListAsync();
    }

    public async Task<SpecialityDto?> GetSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities
            .FirstOrDefaultAsync(s => s.Id == id);

        if (speciality == null)
            return null;

        return new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        };
    }

    public async Task<(SpecialityDto? Speciality, string? Error)> 
        CreateSpecialityAsync(SpecialityDto dto)
    {
        if (await _context.Specialities
            .AnyAsync(s => s.Name == dto.Name))
            return (null, "Speciality with this name already exists.");

        var speciality = new Speciality
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Specialities.Add(speciality);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create speciality: {ex.InnerException?.Message}");
        }
       
        return (new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        }, null);
    }

    public async Task<(bool Success, string? Error)> 
        UpdateSpecialityAsync(Guid id, SpecialityDto dto)
    {
        var speciality = await _context.Specialities
            .FirstOrDefaultAsync(s => s.Id == dto.Id);

        if (speciality == null)
            return (false, "Speciality not found.");

        if (await _context.Specialities
            .AnyAsync(s => s.Name == dto.Name && s.Id != id))
            return (false, "Speciality with this name already exists.");

        if (dto.Name != null)
            speciality.Name = dto.Name;
        if (dto.Description != null)
            speciality.Description = dto.Description;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update speciality: {ex.InnerException?.Message}");
        }
        
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> 
        DeleteSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities
            .FirstOrDefaultAsync(s => s.Id == id);

        if (speciality == null)
            return (false, "Speciality not found.");

        _context.Specialities.Remove(speciality);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete speciality: {ex.InnerException?.Message}");
        }
        
        return (true, null);
    }
}
