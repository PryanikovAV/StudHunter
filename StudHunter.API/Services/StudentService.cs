using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class StudentService(StudHunterDbContext context, IPasswordHasher passwordHasher) : BaseEntityService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<StudentDto?> GetStudentAsync(Guid id)
    {
        var student = await _context.Students
            .Include(s => s.Resume)
            .Include(s => s.StudyPlan)
            .Include(s => s.Achievements)
            .ThenInclude(ua => ua.AchievementTemplate)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return null;

        return new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Gender = student.Gender.ToString(),
            BirthDate = student.BirthDate,
            Photo = student.Photo,
            ContactPhone = student.ContactPhone,
            ContactEmail = student.ContactEmail,
            IsForeign = student.IsForeign,
            StatusId = student.StatusId,
            ResumeId = student.Resume != null ? student.Resume.Id : null,
            CreatedAt = student.CreatedAt,
            CourseNumber = student.StudyPlan.CourseNumber,
            FacultyId = student.StudyPlan.FacultyId,
            SpecialityId = student.StudyPlan.SpecialityId,
            StudyForm = student.StudyPlan.StudyForm.ToString(),
            BeginYear = student.StudyPlan.BeginYear,
            Achievements = student.Achievements.Select(userAchievement => new UserAchievementDto
            {
                UserId = userAchievement.UserId,
                AchievementTemplateId = userAchievement.AchievementTemplateId,
                AchievementAt = userAchievement.AchievementAt,
                AchievementName = userAchievement.AchievementTemplate.Name,
                AchievementDescription = userAchievement.AchievementTemplate.Description
            }).ToList()
        };
    }

    public async Task<(StudentDto? Student, string? Error)> CreateStudentAsync(CreateStudentDto dto)
    {
        if (await _context.Students.AnyAsync(s => s.Email == dto.Email))
            return (null, "Student with this email already exists");

        if (!await _context.Faculties.AnyAsync(f => f.Id == dto.FacultyId))
            return (null, "Faculty not found");

        if (!await _context.Specialities.AnyAsync(s => s.Id == dto.SpecialityId))
            return (null, "Speciality not found");

        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Gender = Enum.Parse<Student.StudentGender>(dto.Gender),
            BirthDate = dto.BirthDate,
            Photo = dto.Photo,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            IsForeign = dto.IsForeign,
            StatusId = dto.StatusId,
            CreatedAt = DateTime.UtcNow
        };

        var studyPlan = new StudyPlan
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            Student = student,
            CourseNumber = dto.CourseNumber,
            FacultyId = dto.FacultyId,
            SpecialityId = dto.SpecialityId,
            StudyForm = Enum.Parse<StudyPlan.StudyForms>(dto.StudyForm),
            BeginYear = dto.BeginYear
        };

        _context.Students.Add(student);
        _context.StudyPlans.Add(studyPlan);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create student: {ex.InnerException?.Message}");
        }

        return (new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Gender = student.Gender.ToString(),
            BirthDate = student.BirthDate,
            Photo = student.Photo,
            ContactPhone = student.ContactPhone,
            ContactEmail = student.ContactEmail,
            IsForeign = student.IsForeign,
            StatusId = student.StatusId,
            ResumeId = null,
            CreatedAt = student.CreatedAt,
            CourseNumber = studyPlan.CourseNumber,
            FacultyId = studyPlan.FacultyId,
            SpecialityId = studyPlan.SpecialityId,
            StudyForm = studyPlan.StudyForm.ToString(),
            BeginYear = studyPlan.BeginYear
        }, null);
    }

    public async Task<(bool Success, string? Error)> UpdateStudentAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _context.Students
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return (false, "Student not found");

        if (dto.Email != null && await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != id))
            return (false, "Another student with this email already exists");

        if (dto.FacultyId.HasValue && !await _context.Faculties.AnyAsync(f => f.Id == dto.FacultyId.Value))
            return (false, "Faculty not found");

        if (dto.SpecialityId.HasValue && !await _context.Specialities.AnyAsync(s => s.Id == dto.SpecialityId.Value))
            return (false, "Speciality not found");

        if (dto.FirstName != null)
            student.FirstName = dto.FirstName;
        if (dto.LastName != null)
            student.LastName = dto.LastName;
        if (dto.Email != null)
            student.Email = dto.Email;
        if (dto.Password != null)
            student.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        if (dto.Gender != null)
            student.Gender = Enum.Parse<Student.StudentGender>(dto.Gender);
        if (dto.BirthDate.HasValue)
            student.BirthDate = dto.BirthDate.Value;
        if (dto.Photo != null)
            student.Photo = dto.Photo;
        if (dto.ContactPhone != null)
            student.ContactPhone = dto.ContactPhone;
        if (dto.ContactEmail != null)
            student.ContactEmail = dto.ContactEmail;
        if (dto.IsForeign.HasValue)
            student.IsForeign = dto.IsForeign.Value;
        if (dto.StatusId.HasValue)
            student.StatusId = dto.StatusId.Value;

        if (dto.CourseNumber.HasValue || dto.FacultyId.HasValue || dto.SpecialityId.HasValue || dto.StudyForm != null || dto.BeginYear.HasValue)
        {
            var studyPlan = student.StudyPlan;
            if (dto.CourseNumber.HasValue)
                studyPlan.CourseNumber = dto.CourseNumber.Value;
            if (dto.FacultyId.HasValue)
                studyPlan.FacultyId = dto.FacultyId.Value;
            if (dto.SpecialityId.HasValue)
                studyPlan.SpecialityId = dto.SpecialityId.Value;
            if (dto.StudyForm != null)
                studyPlan.StudyForm = Enum.Parse<StudyPlan.StudyForms>(dto.StudyForm);
            if (dto.BeginYear.HasValue)
                studyPlan.BeginYear = dto.BeginYear.Value;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update student: {ex.InnerException?.Message}");
        }
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteStudentAsync(Guid id)
    {
        return await DeleteEntityAsync<Student>(id);
    }
}
