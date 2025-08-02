using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing resumes with administrative privileges.
/// </summary>
public class AdminResumeService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseResumeService(context, userAchievementService)
{
    /// <summary>
    /// Retrieves all resumes.
    /// </summary>
    /// <returns>A tuple containing a list of all resumes, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminResumeDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllResumesAsync()
    {
        var resumes = await _context.Resumes
        .Select(r => new AdminResumeDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            Title = r.Title,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            IsDeleted = r.IsDeleted
        })
        .ToListAsync();

        return (resumes, null, null);
    }

    /// <summary>
    /// Updates an existing resume.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <param name="dto">The data transfer object containing updated resume details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid id, AdminUpdateResumeDto dto)
    {
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id);

        #region Serializers
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Resume)));
        #endregion

        if (dto.Title != null)
            resume.Title = dto.Title;
        if (dto.Description != null)
            resume.Description = dto.Description;
        if (dto.IsDeleted.HasValue)
            resume.IsDeleted = dto.IsDeleted.Value;
        resume.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Resume>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a resume (hard or soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteResumeAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Resume>(id, hardDelete);
    }
}
