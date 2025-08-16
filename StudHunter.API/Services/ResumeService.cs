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
public class ResumeService(StudHunterDbContext context) : BaseResumeService(context)
{
    /// <summary>
    /// Retrieves a resume by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple containing the resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> GetResumeAsync(Guid id)
    {
        var resume = await _context.Resumes.FindAsync(id);

        if (resume == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));

        if (resume.IsDeleted)
            return (null, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Resume)));

        return (MapToResumeDto<ResumeDto>(resume), null, null);
    }

    /// <summary>
    /// Creates a new resume for a student.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing resume details.</param>
    /// <returns>A tuple containing the created resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> CreateResumeAsync(Guid id, CreateResumeDto dto)
    {
        var student = await _context.Students
        .Include(s => s.StudyPlan)
        .Include(r => r.Resume)
        .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));

        if (student.Resume != null && !student.Resume.IsDeleted)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Resume), "studentId"));

        if (student.StudyPlan == null)
            return (null, StatusCodes.Status400BadRequest, $"Fill out the {nameof(StudyPlan)} to create {nameof(Resume)}");

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

        return (MapToResumeDto<ResumeDto>(resume), null, null);
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

        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));

        if (resume.IsDeleted)
            return (false, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Resume)));

        if (dto.Title != null)
            resume.Title = dto.Title;
        if (dto.Description != null)
            resume.Description = dto.Description;
        resume.UpdatedAt = DateTime.UtcNow;

        return await SaveChangesAsync<Resume>();
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
