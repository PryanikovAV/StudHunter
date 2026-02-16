using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IStudyPlanService
{
    Task<Result<StudyPlanDto>> GetStudyPlanByStudentIdAsync(Guid studentId);
    Task<Result<StudyPlanDto>> UpdateStudyPlanAsync(Guid studentId, UpdateStudyPlanDto dto);
}

public class StudyPlanService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseStudyPlanService(context, registrationManager), IStudyPlanService
{
    public virtual async Task<Result<StudyPlanDto>> GetStudyPlanByStudentIdAsync(Guid studentId)
    {
        var studyPlan = await GetFullStudyPlanQuery()
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId);

        if (studyPlan == null)
        {
            studyPlan = new StudyPlan { StudentId = studentId };
            _context.StudyPlans.Add(studyPlan);
            await _context.SaveChangesAsync();

            return Result<StudyPlanDto>.Success(StudyPlanMapper.ToDto(studyPlan));
        }

        if (studyPlan.IsDeleted)
        {
            studyPlan.IsDeleted = false;
            studyPlan.DeletedAt = null;
            await _context.SaveChangesAsync();
        }

        return Result<StudyPlanDto>.Success(StudyPlanMapper.ToDto(studyPlan));
    }

    public async Task<Result<StudyPlanDto>> UpdateStudyPlanAsync(Guid studentId, UpdateStudyPlanDto dto)
    {
        var studyPlan = await GetFullStudyPlanQuery()
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId);

        if (studyPlan == null)
        {
            studyPlan = new StudyPlan { StudentId = studentId };
            _context.StudyPlans.Add(studyPlan);
        }

        if (studyPlan.IsDeleted)
        {
            studyPlan.IsDeleted = false;
            studyPlan.DeletedAt = null;
        }

        StudyPlanMapper.ApplyUpdate(studyPlan, dto);

        var result = await SaveChangesAsync<StudyPlan>();

        if (!result.IsSuccess)
            return Result<StudyPlanDto>.Failure(result.ErrorMessage!);

        return Result<StudyPlanDto>.Success(StudyPlanMapper.ToDto(studyPlan));
    }
}