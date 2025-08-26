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
    /// Retrieves a resume by studentID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="authUserId">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple containing the resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> GetResumeAsync(Guid studentId, Guid authUserId)
    {
        var resume = await _context.Resumes.FindAsync(studentId);
        if (resume == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        if (resume.IsDeleted && resume.StudentId != authUserId)
            return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", nameof(Resume)));
        return (MapToResumeDto<ResumeDto>(resume), null, null);
    }

    /// <summary>
    /// Creates a new resume for a student.
    /// </summary>
    /// <param name="authUserId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing resume details.</param>
    /// <returns>A tuple containing the created resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> CreateResumeAsync(Guid authUserId, CreateResumeDto dto)
    {
        var student = await _context.Students.FindAsync(authUserId);
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.IsDeleted)
            return (null, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Student)));
        if (student.Resume != null)
            return (null, StatusCodes.Status409Conflict, "Student already has a resume. Use UpdateResumeStatus to restore.");
        if (student.StudyPlan == null)
            return (null, StatusCodes.Status400BadRequest, $"Fill out the {nameof(StudyPlan)} to create {nameof(Resume)}");

        var resume = new Resume
        {
            Id = Guid.NewGuid(),
            StudentId = authUserId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
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
    /// <param name="resumeId"></param>
    /// <param name="authUserId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid authUserId, Guid resumeId, UpdateResumeDto dto)
    {
        var resume = await _context.Resumes.FindAsync(resumeId);
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        if (resume.StudentId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", nameof(Resume)));
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
    /// 
    /// </summary>
    /// <param name="resumeId"></param>
    /// <param name="authUserId"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeStatusAsync(Guid authUserId, Guid resumeId, bool isDeleted)
    {
        var resume = await _context.Resumes.FindAsync(resumeId);
        if (resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        if (resume.StudentId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update status", nameof(Resume)));

        resume.IsDeleted = isDeleted;
        resume.DeletedAt = isDeleted ? DateTime.UtcNow : null;

        if (isDeleted)
        {
            var invitations = await _context.Invitations
                .Where(i => i.ResumeId == resumeId && i.Status != Invitation.InvitationStatus.Rejected)
                .ToListAsync();
            foreach (var invitation in invitations)
            {
                invitation.Status = Invitation.InvitationStatus.Rejected;
                invitation.UpdatedAt = DateTime.UtcNow;
            }
        }
        return await SaveChangesAsync<Resume>();
    }
}
