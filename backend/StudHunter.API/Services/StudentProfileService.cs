using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IStudentProfileService
{
    Task<Result<StudentProfileDto>> GetProfileAsync(Guid studentId);
    Task<Result<StudentProfileDto>> UpdateProfileAsync(Guid studentId, StudentProfileDto dto);
}

public class StudentProfileService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseStudentService(context, registrationManager), IStudentProfileService
{
    public async Task<Result<StudentProfileDto>> GetProfileAsync(Guid studentId)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.StudyPlan)
            .ThenInclude(sp => sp!.StudyPlanCourses)
            .ThenInclude(spc => spc.Course)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return Result<StudentProfileDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        return Result<StudentProfileDto>.Success(StudentProfileMapper.ToProfileDto(student));
    }

    public async Task<Result<StudentProfileDto>> UpdateProfileAsync(Guid studentId, StudentProfileDto dto)
    {
        var student = await _context.Students
            .Include(s => s.Resume)
            .Include(s => s.StudyPlan)
                .ThenInclude(sp => sp!.StudyPlanCourses)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return Result<StudentProfileDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        if (student.StudyPlan == null)
        {
            var newPlan = new StudyPlan
            {
                StudentId = studentId,
                StudyPlanCourses = new List<StudyPlanCourse>()
            };
            _context.StudyPlans.Add(newPlan);
            student.StudyPlan = newPlan;
        }
        else if (student.StudyPlan.IsDeleted)
        {
            student.StudyPlan.IsDeleted = false;
            student.StudyPlan.DeletedAt = null;
        }

        StudentProfileMapper.ApplyProfileUpdate(student, dto, student.StudyPlan);

        _registrationManager.RecalculateRegistrationStage(student);

        var result = await SaveChangesAsync<Student>();

        if (!result.IsSuccess)
            return Result<StudentProfileDto>.Failure(result.ErrorMessage!);

        return Result<StudentProfileDto>.Success(StudentProfileMapper.ToProfileDto(student));
    }
}
