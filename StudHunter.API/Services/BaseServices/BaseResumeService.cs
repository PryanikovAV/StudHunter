using StudHunter.API.ModelsDto.ResumeDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseResumeService(StudHunterDbContext context) : BaseService(context)
{
    protected static ResumeDto MapToResumeDto(Resume resume)
    {
        return new ResumeDto
        {
            Id = resume.Id,
            StudentId = resume.StudentId,
            Title = resume.Title,
            Description = resume.Description,
            CreatedAt = resume.CreatedAt,
            UpdatedAt = resume.UpdatedAt
        };
    }
}
