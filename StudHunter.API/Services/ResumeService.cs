using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class ResumeService(StudHunterDbContext context) : BaseEntityService(context)
{
    public async Task<IEnumerable<ResumeDto>> GetAllResumesAsync()
    {
        return await _context.Resumes
            .Select(r => new ResumeDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            Title = r.Title,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        })
            .ToListAsync();
    }

    public async Task<ResumeDto?> GetResumeAsync(Guid id)
    {
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id);
        
        if (resume == null)
            return null;
        
        return new ResumeDto
        {
            Id = resume.Id,
            StudentId = resume.StudentId,
            Title = resume.Title,
            Description = resume.Description,
            CreatedAt = resume.CreatedAt,
            UpdatedAt = resume.UpdatedAt
        };
    }

    public async Task<(ResumeDto? Resume, string? Error)> CreateResumeAsync(CreateResumeDto dto)
    {
        if (!await _context.Resumes.AnyAsync(s => s.Id == dto.StudentId))
            return (null, "Student not found.");

        if (await _context.Resumes.AnyAsync(r => r.StudentId == dto.StudentId))
            return (null, "Resume for this student already exists.");

        var resume = new Resume
        {
            Id = Guid.NewGuid(),
            StudentId = dto.StudentId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Resumes.Add(resume);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create resume: {ex.InnerException?.Message}");
        }

        return (new ResumeDto
        {
            Id = resume.Id,
            StudentId = resume.StudentId,
            Title = resume.Title,
            Description = resume.Description,
            CreatedAt = resume.CreatedAt,
            UpdatedAt = resume.UpdatedAt
        }, null);
    }

    public async Task<(bool Success, string? Error)> UpdateResumeAsync(Guid id, UpdateResumeDto dto)
    {
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id);
        
        if (resume == null)
            return (false, "Resume not found.");

        if (dto.Title != null)
            resume.Title = dto.Title;
        if (dto.Description != null)
            resume.Description = dto.Description;
        resume.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update resume: {ex.InnerException?.Message}");
        }
        return (true, null);
    }
}
