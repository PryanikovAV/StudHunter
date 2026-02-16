using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminStudyPlanService : IStudyPlanService
{
    Task<Result<StudyPlanDto>> CreateAsync(Guid studentId, UpdateStudyPlanDto dto);
    Task<Result<bool>> DeleteAsync(Guid id, bool hardDelete);
}

public class AdminStudyPlanService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : StudyPlanService(context, registrationManager), IAdminStudyPlanService
{
    public async Task<Result<StudyPlanDto>> CreateAsync(Guid studentId, UpdateStudyPlanDto dto)
    {
        var existingPlan = await _context.StudyPlans
            .IgnoreQueryFilters()
            .AnyAsync(sp => sp.StudentId == studentId);

        if (existingPlan)
            return Result<StudyPlanDto>.Failure(ErrorMessages.AlreadyExists(nameof(StudyPlan)), StatusCodes.Status409Conflict);

        var studyPlan = new StudyPlan { StudentId = studentId };
        StudyPlanMapper.ApplyUpdate(studyPlan, dto);

        _context.StudyPlans.Add(studyPlan);
        var result = await SaveChangesAsync<StudyPlan>();

        return result.IsSuccess
            ? await GetStudyPlanByStudentIdAsync(studentId)
            : Result<StudyPlanDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, bool hardDelete)
    {
        var studyPlan = await _context.StudyPlans
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(sp => sp.Id == id);

        if (studyPlan == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(StudyPlan)), StatusCodes.Status404NotFound);

        if (hardDelete)
            _context.StudyPlans.Remove(studyPlan);
        else
        {
            studyPlan.IsDeleted = true;
            studyPlan.DeletedAt = DateTime.UtcNow;
        }

        return await SaveChangesAsync<StudyPlan>();
    }
}