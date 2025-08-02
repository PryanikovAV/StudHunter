using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing resumes.
/// </summary>
public class ResumeService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseResumeService(context, userAchievementService)
{
    /// <summary>
    /// Creates a new resume for a student.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing resume details.</param>
    /// <returns>A tuple containing the created resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> CreateResumeAsync(Guid id, CreateResumeDto dto)
    {
        #region Serializers
        if (!await _context.Users.AnyAsync(u => u.Id == id && u is Student))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Student)));

        if (await _context.Resumes.AnyAsync(r => r.StudentId == id && !r.IsDeleted))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Resume), "Student Id"));
        #endregion

        var resume = new Resume
        {
            Id = Guid.NewGuid(),
            StudentId = id,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Resumes.Add(resume);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Resume>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new ResumeDto
        {
            Id = resume.Id,
            StudentId = resume.StudentId,
            Title = resume.Title,
            Description = resume.Description,
            CreatedAt = resume.CreatedAt,
            UpdatedAt = resume.UpdatedAt
        }, null, null);
    }

    /// <summary>
    /// Updates an existing resume.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <param name="dto">The data transfer object containing updated resume details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid id, UpdateResumeDto dto)
    {
        var resume = await _context.Resumes.FindAsync(id);

        #region Serializers
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Resume)));

        if (resume.IsDeleted)
            return (false, StatusCodes.Status410Gone, ErrorMessages.AlreadyDeleted(nameof(Resume)));
        #endregion

        if (dto.Title != null)
            resume.Title = dto.Title;
        if (dto.Description != null)
            resume.Description = dto.Description;
        resume.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Resume>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a resume (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteResumeAsync(Guid id)
    {
        return await DeleteEntityAsync<Resume>(id, hardDelete: false);
    }
}
