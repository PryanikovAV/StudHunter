using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public class BaseResumeService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

    /// <summary>
    /// Retrieves a resume by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple containing the resume, an optional status code, and an optional error message.</returns>
    public async Task<(ResumeDto? Entity, int? StatusCode, string? ErrorMessage)> GetResumeAsync(Guid id)
    {
        var resume = await _context.Resumes.FindAsync(id);

        #region Serializers
        if (resume == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Resume)));

        if (resume.IsDeleted)
            return (null, StatusCodes.Status410Gone, ErrorMessages.AlreadyDeleted(nameof(Resume)));
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
}
