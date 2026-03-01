using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudHunter.API.Services.AuthService;

public interface IAuthService
{
    Task<Result<AuthDto>> LoginAsync(LoginDto dto);
    Task<Result<AuthDto>> RegisterStudentAsync(RegisterStudentDto dto);
    Task<Result<AuthDto>> RegisterEmployerAsync(RegisterEmployerDto dto);
    Task<Result<AuthDto>> RecoverAccountAsync(LoginDto dto);
}

public class AuthService(StudHunterDbContext context,
    IPasswordHasher passwordHasher,
    IConfiguration configuration,
    IRegistrationManager registrationManager)
    : BaseService(context, registrationManager), IAuthService
{
    public async Task<Result<AuthDto>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            return Result<AuthDto>.Failure(ErrorMessages.InvalidCredentials(), StatusCodes.Status401Unauthorized);

        return CreateAuthResult(user);
    }

    public async Task<Result<AuthDto>> RegisterStudentAsync(RegisterStudentDto dto)
    {
        if (await UserExists(dto.Email))
            return Result<AuthDto>.Failure(ErrorMessages.AlreadyExists(nameof(User), nameof(User.Email)), StatusCodes.Status409Conflict);

        var student = new Student
        {
            Email = dto.Email,
            PasswordHash = passwordHasher.HashPassword(dto.Password),
            FirstName = UserDefaultNames.DefaultFirstName,
            LastName = UserDefaultNames.DefaultLastName
        };

        _context.Students.Add(student);
        var result = await SaveChangesAsync<Student>();

        return result.IsSuccess
            ? CreateAuthResult(student)
            : Result<AuthDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<AuthDto>> RegisterEmployerAsync(RegisterEmployerDto dto)
    {
        if (await UserExists(dto.Email))
            return Result<AuthDto>.Failure(ErrorMessages.AlreadyExists(nameof(User), nameof(User.Email)), StatusCodes.Status409Conflict);

        var employer = new Employer
        {
            Email = dto.Email,
            PasswordHash = passwordHasher.HashPassword(dto.Password),
            Name = string.IsNullOrWhiteSpace(dto.Name) ? UserDefaultNames.DefaultCompanyName : dto.Name
        };

        _context.Employers.Add(employer);
        var result = await SaveChangesAsync<Employer>();

        return result.IsSuccess
            ? CreateAuthResult(employer)
            : Result<AuthDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<AuthDto>> RecoverAccountAsync(LoginDto dto)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            return Result<AuthDto>.Failure(ErrorMessages.InvalidCredentials(), StatusCodes.Status401Unauthorized);

        if (!user.IsDeleted)
            return Result<AuthDto>.Failure(ErrorMessages.AccountAlreadyActive(), StatusCodes.Status400BadRequest);

        if (user is Administrator)
            return Result<AuthDto>.Failure(ErrorMessages.AdminSelfRecovery(), StatusCodes.Status403Forbidden);

        if (user is Student student)
        {
            await _context.Entry(student).Reference(s => s.Resume).Query().IgnoreQueryFilters().LoadAsync();
            await _context.Entry(student).Reference(s => s.StudyPlan).Query().IgnoreQueryFilters().LoadAsync();
        }
        else if (user is Employer employer)
        {
            await _context.Entry(employer).Collection(e => e.Vacancies).Query().IgnoreQueryFilters().LoadAsync();
        }

        var deletedAt = user.DeletedAt;
        user.IsDeleted = false;
        user.DeletedAt = null;

        RestoreRelatedEntities(user, deletedAt);

        var result = await SaveChangesAsync<User>();
        return result.IsSuccess
            ? CreateAuthResult(user)
            : Result<AuthDto>.Failure(ErrorMessages.RecoveryFailed());
    }

    private async Task<bool> UserExists(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);

    private Result<AuthDto> CreateAuthResult(User user)
    {
        var role = GetRole(user);
        var token = GenerateJwtToken(user.Id, role);
        return Result<AuthDto>.Success(new AuthDto(user.Id, user.Email, role, token));
    }

    private string GenerateJwtToken(Guid userId, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),  // TODO: изменить период действия токена
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void RestoreRelatedEntities(User user, DateTime? accountDeletedAt)
    {
        if (!accountDeletedAt.HasValue) return;

        var threshold = accountDeletedAt.Value.AddSeconds(-10);  // окно допуска в 10 сек

        if (user is Student student)
        {
            if (student.Resume != null && student.Resume.IsDeleted && student.Resume.DeletedAt >= threshold)
            {
                student.Resume.IsDeleted = false;
                student.Resume.DeletedAt = null;
            }
            if (student.StudyPlan != null && student.StudyPlan.IsDeleted && student.StudyPlan.DeletedAt >= threshold)
            {
                student.StudyPlan.IsDeleted = false;
                student.StudyPlan.DeletedAt = null;
            }
        }
        else if (user is Employer employer)
        {
            foreach (var v in employer.Vacancies.Where(v => v.IsDeleted && v.DeletedAt >= threshold))
            {
                v.IsDeleted = false;
                v.DeletedAt = null;
            }
        }
    }
}