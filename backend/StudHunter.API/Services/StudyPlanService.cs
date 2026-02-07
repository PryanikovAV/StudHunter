using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IStudyPlanService
{
    Task<Result<StudyPlanDto>> GetByStudentIdAsync(Guid studentId);
    Task<Result<StudyPlanDto>> UpdateAsync(Guid studentId, UpdateStudyPlanDto dto);
}

public class StudyPlanService(StudHunterDbContext context) : BaseService(context), IStudyPlanService
{
    public virtual async Task<Result<StudyPlanDto>> GetByStudentIdAsync(Guid studentId)
    {
        var studyPlan = await _context.StudyPlans
            .IgnoreQueryFilters()
            .Include(sp => sp.University)
            .Include(sp => sp.Faculty)
            .Include(sp => sp.Department)
            .Include(sp => sp.StudyDirection)
            .Include(sp => sp.StudyPlanCourses)
            .ThenInclude(spc => spc.Course)
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId);

        if (studyPlan == null)
            return await CreateInternalAsync(studentId);

        if (studyPlan.IsDeleted)
        {
            studyPlan.IsDeleted = false;
            studyPlan.DeletedAt = null;
            await _context.SaveChangesAsync();
        }

        return Result<StudyPlanDto>.Success(StudyPlanMapper.ToDto(studyPlan));
    }

    protected async Task<Result<StudyPlanDto>> CreateInternalAsync(Guid studentId)
    {
        var studyPlan = new StudyPlan
        {
            StudentId = studentId
        };

        _context.StudyPlans.Add(studyPlan);
        await _context.SaveChangesAsync();

        return await GetByStudentIdAsync(studentId);
    }

    public async Task<Result<StudyPlanDto>> UpdateAsync(Guid studentId, UpdateStudyPlanDto dto)
    {
        var studyPlan = await _context.StudyPlans
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId);

        if (studyPlan == null)
        {
            var createResult = await CreateInternalAsync(studentId);
            if (!createResult.IsSuccess)
                return createResult;
        }

        studyPlan = await _context.StudyPlans.FirstAsync(sp => sp.StudentId == studentId);

        if (studyPlan.IsDeleted)
        {
            studyPlan.IsDeleted = false;
            studyPlan.DeletedAt = null;
        }

        StudyPlanMapper.ApplyUpdate(studyPlan, dto);

        var result = await SaveChangesAsync<StudyPlan>();

        return result.IsSuccess
            ? await GetByStudentIdAsync(studentId)
            : Result<StudyPlanDto>.Failure(result.ErrorMessage!);
    }
}