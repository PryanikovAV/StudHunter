using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.ResumeDto;
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
    public async Task<(List<ResumeDto> Entities, int? StatusCode, string? ErrorMessage)> GetAllResumesAsync()
    {
        var resumes = await _context.Resumes
            .IgnoreQueryFilters()
            .Select(r => MapToResumeDto(r))
            .ToListAsync();
        return (resumes, null, null);
    }

    /// <summary>
    /// Retrieves a resume by studentId.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>A tuple containing the resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> GetResumeAsync(Guid studentId)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(r => r.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Resume == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));

        return (MapToResumeDto(student.Resume), null, null);
    }

    /// <summary>
    /// Updates an existing resume.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the resume.</param>
    /// <param name="dto">The data transfer object containing updated resume details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid studentId, UpdateResumeDto dto)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(r => r.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));

        if (dto.Title != null)
            student.Resume.Title = dto.Title;
        if (dto.Description != null)
            student.Resume.Description = dto.Description;

        student.Resume.UpdatedAt = DateTime.UtcNow;
        return await SaveChangesAsync<Resume>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeStatusAsync(Guid studentId, UpdateStatusDto dto)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(r => r.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));

        student.Resume.IsDeleted = dto.IsDeleted;
        student.Resume.DeletedAt = dto.IsDeleted ? DateTime.UtcNow : null;

        if (dto.IsDeleted)
        {
            var invitations = await _context.Invitations
                .Where(i => i.ResumeId == studentId && i.Status != Invitation.InvitationStatus.Rejected)
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
    /// <param name="studentId">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteResumeAsync(Guid studentId)
    {
        var student = await _context.Students
            .IgnoreQueryFilters()
            .Include(r => r.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));

        _context.Resumes.Remove(student.Resume);
        return await SaveChangesAsync<Resume>();
    }
}
