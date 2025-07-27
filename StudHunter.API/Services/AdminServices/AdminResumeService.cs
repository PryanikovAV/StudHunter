using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminResumeService(StudHunterDbContext context, UserAchievementService userAchievementService)
: ResumeService(context, userAchievementService)
{
    public async Task<(List<AdminResumeDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllResumesAsync()
    {
        var resumes = await _context.Resumes.Select(r => new AdminResumeDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            Title = r.Title,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            IsDeleted = r.IsDeleted
        }).ToListAsync();

        return (resumes, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid id, AdminUpdateResumeDto dto)
    {
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id);

        #region Serializers
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Resume"));
        #endregion

        if (dto.Title != null)
            resume.Title = dto.Title;
        if (dto.Description != null)
            resume.Description = dto.Description;
        if (dto.IsDeleted.HasValue)
            resume.IsDeleted = dto.IsDeleted.Value;
        resume.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Resume");

        return (success, statusCode, errorMessage);
    }

    public override async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid id, UpdateResumeDto dto)
    {
        return await Task.FromException<(bool Success, int? StatusCode, string? ErrorMessage)>(
        new NotSupportedException("Admins must use AdminUpdateResumeDto."));

    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteResumeAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Resume>(id, hardDelete);
    }

    public override async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteResumeAsync(Guid id)
    {
        return await DeleteEntityAsync<Resume>(id, hardDelete: true);
    }
}
