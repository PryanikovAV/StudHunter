using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.ResumeDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing resumes.
/// </summary>
public class ResumeService(StudHunterDbContext context, StudyPlanService studyPlanService) : BaseResumeService(context)
{
    private readonly StudyPlanService _studyPlanService = studyPlanService;

    /// <summary>
    /// Retrieves a resume by studentID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="authUserId">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple containing the resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> GetResumeAsync(Guid studentId, Guid authUserId)
    {
        if (studentId != authUserId)
            return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", nameof(Resume)));
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Resume == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));

        return (MapToResumeDto(student.Resume), null, null);
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
        var resume = await _context.Resumes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == student.Id);
        if (resume != null && resume.IsDeleted)
            return (null, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Resume), nameof(UpdateResumeStatusAsync)));
        if (resume != null)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Student), nameof(Resume)));
        var (studyPlan, studyPlanStatusCode, studyPlanErrorMessage) = await _studyPlanService.GetStudyPlanByStudentAsync(authUserId, authUserId);
        if (studyPlan == null)
            return (null, StatusCodes.Status400BadRequest, $"{nameof(StudyPlan)} is required to create a {nameof(Resume)}. Please create a {nameof(StudyPlan)} first.");

        resume = new Resume
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

        return (MapToResumeDto(resume), null, null);
    }

    /// <summary>
    /// Updates an existing resume.
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="authUserId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid authUserId, Guid studentId, UpdateResumeDto dto)
    {
        if (studentId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", nameof(Resume)));
        var student = await _context.Students
            .Include(r => r.Resume)
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));
        if (student.Resume == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
        if (await _context.Resumes.IgnoreQueryFilters().AnyAsync(r => r.Id == student.Resume.Id))
            return (false, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Resume), nameof(UpdateResumeStatusAsync)));

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
    /// <param name="authUserId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeStatusAsync(Guid authUserId, Guid studentId, UpdateStatusDto dto)
    {
        if (studentId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update status", nameof(Resume)));
        var student = await _context.Students
            .IgnoreAutoIncludes()
            .Include(s => s.Resume)
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
}
