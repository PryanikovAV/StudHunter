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
public class AdminResumeService(StudHunterDbContext context) : BaseResumeService(context)
{
    /// <summary>
    /// Retrieves all resumes include deleted.
    /// </summary>
    /// <returns>A tuple containing a list of all resumes, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminResumeDto> Entities, int? StatusCode, string? ErrorMessage)> GetAllResumesAsync()
    {
        var resumes = await _context.Resumes
        .Select(r => MapToResumeDto<AdminResumeDto>(r))
        .ToListAsync();
        return (resumes, null, null);
    }

    /// <summary>
    /// Retrieves a resume by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple containing the resume, an optional status code, and an optional error message.</returns>
    public async Task<(AdminResumeDto? Entity, int? StatusCode, string? ErrorMessage)> GetResumeAsync(Guid id)
    {
        var resume = await _context.Resumes
        .FirstOrDefaultAsync(r => r.Id == id);
        if (resume == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        return (MapToResumeDto<AdminResumeDto>(resume), null, null);
    }

    /// <summary>
    /// Updates an existing resume.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <param name="dto">The data transfer object containing updated resume details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid id, AdminUpdateResumeDto dto)
    {
        var resume = await _context.Resumes
        .FirstOrDefaultAsync(r => r.Id == id);
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        if (dto.Title != null)
            resume.Title = dto.Title;
        if (dto.Description != null)
            resume.Description = dto.Description;
        resume.UpdatedAt = DateTime.UtcNow;
        return await SaveChangesAsync<Resume>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeStatusAsync(Guid id, bool isDeleted)
    {
        var resume = await _context.Resumes
        .FirstOrDefaultAsync(r => r.Id == id);
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        resume.IsDeleted = isDeleted;
        resume.DeletedAt = isDeleted ? DateTime.UtcNow : null;

        if (isDeleted)
        {
            var invitations = await _context.Invitations
            .Where(i => i.ResumeId == id && i.Status != Invitation.InvitationStatus.Rejected)
            .ToListAsync();
            foreach (var invitation in invitations)
            {
                invitation.Status = Invitation.InvitationStatus.Rejected;
                invitation.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await SaveChangesAsync<Resume>();
    }

    /// <summary>
    /// Deletes a resume.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteResumeAsync(Guid id)
    {
        var resume = await _context.Resumes.FindAsync(id);
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        _context.Resumes.Remove(resume);
        return await SaveChangesAsync<Resume>();
    }
}
