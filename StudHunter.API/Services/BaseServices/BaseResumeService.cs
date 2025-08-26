using StudHunter.API.ModelsDto.Resume;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseResumeService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Maps a Resume entity to a specified DTO type.
    /// </summary>
    /// <typeparam name="TDto">The type of DTO to map to (must inherit from ResumeDto).</typeparam>
    /// <param name="resume">The Resume entity to map.</param>
    /// <returns>The mapped DTO.</returns>
    protected TDto MapToResumeDto<TDto>(Resume resume) where TDto : ResumeDto, new()
    {
        var resumeDto = new TDto
        {
            Id = resume.Id,
            StudentId = resume.StudentId,
            Title = resume.Title,
            Description = resume.Description,
            CreatedAt = resume.CreatedAt,
            UpdatedAt = resume.UpdatedAt
        };

        if (resumeDto is AdminResumeDto adminResumeDto)
        {
            adminResumeDto.IsDeleted = resume.IsDeleted;
        }

        return resumeDto;
    }
}
