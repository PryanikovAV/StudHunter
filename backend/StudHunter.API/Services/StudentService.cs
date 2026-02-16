using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AuthService;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IStudentService
{
    Task<Result<StudentDto>> GetStudentAsync(Guid studentId);
    Task<Result<StudentHeroDto>> GetStudentHeroAsync(Guid studentId);
    Task<Result<StudentDto>> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto);
    Task<Result<bool>> DeleteStudentAsync(Guid studentId);
}

public class StudentService(StudHunterDbContext context,
    IStudyPlanService studyPlanService,
    IPasswordHasher passwordHasher,
    IRegistrationManager registrationManager)
    : BaseStudentService(context, registrationManager), IStudentService
{
    public async Task<Result<StudentDto>> GetStudentAsync(Guid studentId)
    {
        var student = await GetStudentInternalAsync(studentId);

        if (student == null)
            return Result<StudentDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        return Result<StudentDto>.Success(StudentMapper.ToDto(student));
    }

    public async Task<Result<StudentHeroDto>> GetStudentHeroAsync(Guid studentId)
    {
        var student = await GetStudentInternalAsync(studentId);

        if (student == null)
            return Result<StudentHeroDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        var studyPlan = await studyPlanService.GetStudyPlanByStudentIdAsync(studentId);
        
        var studyPlanDto = studyPlan.IsSuccess ? studyPlan.Value : null;

        return Result<StudentHeroDto>.Success(StudentMapper.ToHeroDto(student, studyPlanDto));
    }

    public async Task<Result<StudentDto>> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto)
    {
        var student = await GetStudentInternalAsync(studentId);

        if (student == null)
            return Result<StudentDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        StudentMapper.ApplyUpdate(student, dto);

        if (!string.IsNullOrWhiteSpace(dto.Password))
            student.PasswordHash = passwordHasher.HashPassword(dto.Password);

        _registrationManager.RecalculateRegistrationStage(student);

        var result = await SaveChangesAsync<Student>();

        return result.IsSuccess
            ? Result<StudentDto>.Success(StudentMapper.ToDto(student))
            : Result<StudentDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> DeleteStudentAsync(Guid studentId)
    {
        var student = await GetStudentInternalAsync(studentId, ignoreFilters: true);

        if (student == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        if (student.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.EntityAlreadyDeleted(nameof(Student)), StatusCodes.Status400BadRequest);

        var now = DateTime.UtcNow;

        await SoftDeleteStudentAsync(student, now);

        return await SaveChangesAsync<Student>();
    }
}