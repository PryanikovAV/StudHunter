using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class StudentService(StudHunterDbContext context, IPasswordHasher passwordHasher) : BaseService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    /// <summary>
    /// Retrieves a student by their ID, including related resume, study plan and achievements.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>The student's detaild or null if not found.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> GetStudentAsync(Guid id)
    {
        var student = await _context.Students
        .Include(s => s.Resume)
        .Include(s => s.StudyPlan)
        .Include(s => s.Achievements)
        .ThenInclude(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        #region Serializers
        if (student == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Student"));

        if (student.StudyPlan == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Study plan"));
        #endregion

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
        }, null, null);
    }

    /// <summary>
    /// Creates a new student with associated study plan.
    /// </summary>
    /// <param name="dto">The student data to create, including study plan details.</param>
    /// <returns>A tuple containing the created student's details or null and an error message if creation fails.</returns>
    public async Task<(StudentDto? Entity, int? StatusCode, string? ErrorMessage)> CreateStudentAsync(CreateStudentDto dto)
    {
        #region Serializers
        var emailExists = await _context.Students.FirstOrDefaultAsync(s => s.Email == dto.Email);
        if (emailExists != null)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Student", "Email"));

        var facultyExists = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == dto.FacultyId);
        if (facultyExists == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Faculty"));

        var specialityExists = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == dto.SpecialityId);
        if (specialityExists == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Speciality"));
        #endregion

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

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Student");

        if (!success)
            return (null, statusCode, errorMessage);

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
        }, null, null);
    }

    /// <summary>
    /// Updates an existing student's data, including studt plan details.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student to update.</param>
    /// <param name="dto">The updated student data.</param>
    /// <returns>A tuple indicating whether the update was successful and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateStudentAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _context.Students
        .Include(s => s.StudyPlan)
        .FirstOrDefaultAsync(s => s.Id == id);

        #region Serializers
        if (student == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Student"));

        if (dto.Email != null)
        {
            var emailExists = await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != id);
            if (emailExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Student", "Email"));
        }

        Guid? facultyId = dto.FacultyId;
        Guid? specialityId = dto.SpecialityId;

        if (facultyId.HasValue || specialityId.HasValue)
        {
            var checks = await Task.WhenAll(
            facultyId.HasValue ? _context.Faculties.AnyAsync(f => f.Id == facultyId.Value) : Task.FromResult(true),
            specialityId.HasValue ? _context.Specialities.AnyAsync(s => s.Id == specialityId.Value) : Task.FromResult(true)
            );

            if (facultyId.HasValue && !checks[0])
                return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Faculty"));

            if (specialityId.HasValue && !checks[1])
                return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Speciality"));
        }
        #endregion

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

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Student");

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteStudentAsync(Guid id)
    {
        return await SoftDeleteEntityAsync<Student>(id);
    }
}
