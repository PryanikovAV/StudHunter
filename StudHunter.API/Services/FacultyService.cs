using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class FacultyService(StudHunterDbContext context)
{
    private readonly StudHunterDbContext _context = context;

    public async Task<IEnumerable<FacultyDto>> GetFacultiesAsync()
    {
        return await _context.Faculties
            .Select(f => new FacultyDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description
            })
            .ToListAsync();
    }

    public async Task<FacultyDto?> GetFacultyAsync(Guid id)
    {
        var faculty = await _context.Faculties
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faculty == null)
            return null;

        return new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        };
    }
    
    public async Task<(FacultyDto? Faculty, string? Error)> 
        CreateFacultyAsync(CreateFacultyDto dto)
    {
        if (await _context.Faculties.AnyAsync(f => f.Name == dto.Name))
            return (null, "Faculty with this name already exists.");

        var faculty = new Faculty
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Faculties.Add(faculty);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create faculty: {ex.InnerException?.Message}");
        }
        
        return (new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        }, null);
    }

    public async Task<(bool Success, string? Error)> 
        UpdateFacultyAsync(Guid id, UpdateFacultyDto dto)
    {
        var faculty = await _context.Faculties
            .FirstOrDefaultAsync(f => f.Id == id);
        
        if (faculty == null)
            return (false, "Faculty not found.");

        if (await _context.Faculties
            .AnyAsync(f => f.Name == dto.Name && f.Id != id))
            return (false, "Faculty with this name already exists.");

        if (dto.Name != null)
            faculty.Name = dto.Name;
        if (dto.Description != null)
            faculty.Description = dto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update faculty: {ex.InnerException?.Message}");
        }
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> 
        DeleteFacultyAsync(Guid id)
    {
        var faculty = await _context.Faculties
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faculty == null)
            return (false, "Course not found.");

        _context.Faculties.Remove(faculty);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete faculty: {ex.InnerException?.Message}");
        }

        return (true, null);
    }
}
