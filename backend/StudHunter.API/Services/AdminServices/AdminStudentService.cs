using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminStudentService
{
    Task<Result<List<AdminStudentDto>>> GetAllStudentsAsync();
    Task<Result<AdminStudentDto>> GetStudentByIdAsync(Guid studentId);
    Task<Result<AdminStudentDto>> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto);
    Task<Result<bool>> DeleteStudentAsync(Guid studentId, bool hardDelete);
    Task<Result<bool>> RestoreAsync(Guid studentId);
}

public class AdminStudentService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseStudentService(context, registrationManager), IAdminStudentService
{
    public async Task<Result<List<AdminStudentDto>>> GetAllStudentsAsync()
    {
        var students = await _context.Students
            .IgnoreQueryFilters()
            .Include(s => s.Resume)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        var dtos = students.Select(StudentMapper.ToAdminDto).ToList();

        return Result<List<AdminStudentDto>>.Success(dtos);
    }

    public async Task<Result<AdminStudentDto>> GetStudentByIdAsync(Guid studentId)
    {
        var student = await GetStudentInternalAsync(studentId, ignoreFilters: true);

        if (student == null)
            return Result<AdminStudentDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        return Result<AdminStudentDto>.Success(StudentMapper.ToAdminDto(student));
    }

    public async Task<Result<AdminStudentDto>> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto)
    {
        var student = await GetStudentInternalAsync(studentId, ignoreFilters: true);

        if (student == null)
            return Result<AdminStudentDto>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        StudentMapper.ApplyUpdate(student, dto);

        _registrationManager.RecalculateRegistrationStage(student);

        await _context.SaveChangesAsync();
        return Result<AdminStudentDto>.Success(StudentMapper.ToAdminDto(student));
    }

    public async Task<Result<bool>> DeleteStudentAsync(Guid studentId, bool hardDelete)
    {
        var student = await GetStudentInternalAsync(studentId, ignoreFilters: true);

        if (student == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        if (hardDelete)
        {
            _context.Students.Remove(student);
        }
        else
        {
            if (student.IsDeleted)
                return Result<bool>.Failure(ErrorMessages.EntityAlreadyDeleted(nameof(Student)), StatusCodes.Status400BadRequest);

            await SoftDeleteStudentAsync(student, DateTime.UtcNow);
        }

        return await SaveChangesAsync<Student>();
    }

    public async Task<Result<bool>> RestoreAsync(Guid studentId)
    {
        var student = await GetStudentInternalAsync(studentId, ignoreFilters: true);

        if (student == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Student)), StatusCodes.Status404NotFound);

        if (!student.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.AccountAlreadyActive(), StatusCodes.Status400BadRequest);

        var deletedAt = student.DeletedAt;
        student.IsDeleted = false;
        student.DeletedAt = null;

        if (student.Resume != null && student.Resume.DeletedAt >= deletedAt)
        {
            student.Resume.IsDeleted = false;
            student.Resume.DeletedAt = null;
        }

        _registrationManager.RecalculateRegistrationStage(student);

        return await SaveChangesAsync<Student>();
    }
}