using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AuthService;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IStudentService
{
    Task<Result<StudentDto>> GetStudentProfileAsync(Guid studentId);
    Task<Result<StudentHeroDto>> GetStudentHeroAsync(Guid studentId);
    Task<Result<StudentDto>> UpdateStudentProfileAsync(Guid studentId, UpdateStudentDto dto);
    Task<Result<bool>> ChangePasswordAsync(Guid studentId, ChangePasswordDto dto);
    Task<Result<string>> UpdateAvatarAsync(Guid studentId, ChangeAvatarDto dto);
    Task<Result<bool>> DeleteStudentAsync(Guid studentId, string password);
}

public class StudentService(
    StudHunterDbContext context,
    IPasswordHasher passwordHasher,
    IRegistrationManager registrationManager)
    : BaseStudentService(context, registrationManager), IStudentService
{
    public async Task<Result<StudentDto>> GetStudentProfileAsync(Guid studentId)
    {
        var student = await GetStudentQuery(asNoTracking: true, includeStudyPlanDictionaries: true, includeCourses: true)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return Result<StudentDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);
        
        return Result<StudentDto>.Success(StudentMapper.ToDto(student));
    }

    public async Task<Result<StudentHeroDto>> GetStudentHeroAsync(Guid studentId)
    {
        var student = await GetStudentQuery(asNoTracking: true, includeStudyPlanDictionaries: true)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return Result<StudentHeroDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);
        
        return Result<StudentHeroDto>.Success(StudentMapper.ToHeroDto(student));
    }

    public async Task<Result<StudentDto>> UpdateStudentProfileAsync(Guid studentId, UpdateStudentDto dto)
    {
        var student = await GetStudentQuery(includeStudyPlanDictionaries: true, includeCourses: true)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return Result<StudentDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        StudentMapper.ApplyUpdate(student, dto);
        _registrationManager.RecalculateRegistrationStage(student);

        var result = await SaveChangesAsync<Student>();

        if (!result.IsSuccess)
            return Result<StudentDto>.Failure(result.ErrorMessage!);

        return await GetStudentProfileAsync(studentId);
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid studentId, ChangePasswordDto dto)
    {
        var student = await GetStudentQuery().FirstOrDefaultAsync(s => s.Id == studentId);
        
        if (student == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        if (!passwordHasher.VerifyPassword(dto.CurrentPassword, student.PasswordHash))
            return Result<bool>.Failure("Неверный текущий пароль", StatusCodes.Status400BadRequest);

        student.PasswordHash = passwordHasher.HashPassword(dto.NewPassword);
        
        return await SaveChangesAsync<Student>();
    }

    public async Task<Result<string>> UpdateAvatarAsync(Guid studentId, ChangeAvatarDto dto)
    {
        var student = await GetStudentQuery().FirstOrDefaultAsync(s => s.Id == studentId);
        
        if (student == null)
            return Result<string>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        student.AvatarUrl = dto.AvatarUrl;
        var result = await SaveChangesAsync<Student>();

        return result.IsSuccess ? Result<string>.Success(student.AvatarUrl) : Result<string>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> DeleteStudentAsync(Guid studentId, string password)
    {
        var student = await GetStudentQuery(ignoreFilters: true, includeStudyPlanDictionaries: true)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);
        
        if (student.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.EntityAlreadyDeleted(nameof(Student)), StatusCodes.Status400BadRequest);

        if (!passwordHasher.VerifyPassword(password, student.PasswordHash))
            return Result<bool>.Failure("Неверный пароль", StatusCodes.Status400BadRequest);

        await SoftDeleteStudentAsync(student, DateTime.UtcNow);
        return await SaveChangesAsync<Student>();
    }
}