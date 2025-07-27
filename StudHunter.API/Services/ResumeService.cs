using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class ResumeService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    public UserAchievementService _userAchievementService = userAchievementService;

    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> GetResumeAsync(Guid id)
    {
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id);

        #region Serializers
        if (resume == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Resume"));

        if (resume.IsDeleted)
            return (null, StatusCodes.Status410Gone, ErrorMessages.AlreadyDeleted("Resume"));
        #endregion

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

    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> CreateResumeAsync(Guid studentId, CreateResumeDto dto)
    {
        #region Serializers
        var studentExists = await _context.Resumes.AnyAsync(s => s.Id == studentId);
        if (studentExists == false)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Student"));

        var resumeExists = await _context.Resumes.AnyAsync(r => r.StudentId == studentId);
        if (resumeExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Resume", "Student"));
        #endregion

        var resume = new Resume
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Resumes.Add(resume);

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Resume");

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

    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateResumeAsync(Guid id, UpdateResumeDto dto)
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
        resume.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Resume");

        return (success, statusCode, errorMessage);
    }

    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteResumeAsync(Guid id)
    {
        return await SoftDeleteEntityAsync<Resume>(id);
    }
}
