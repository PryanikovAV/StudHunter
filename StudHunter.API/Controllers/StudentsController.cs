//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using StudHunter.API.Extensions;
//using StudHunter.API.Models;
//using StudHunter.DB.Postgres;
//using StudHunter.DB.Postgres.Models;

//namespace StudHunter.API.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class StudentsController : ControllerBase
//{
//    private readonly StudHunterDbContext _context;

//    public StudentsController(StudHunterDbContext context)
//    {
//        _context = context;
//    }

//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
//    {
//        return await _context.Students
//            .Select(s => new StudentDto
//            {
//                Id = s.Id,
//                FirstName = s.FirstName,
//                LastName = s.LastName,
//                Email = s.Email,
//                Gender = s.Gender.GetDisplayName(),
//                BirthDate = s.BirthDate,
//                Photo = s.Photo,
//                ContactPhone = s.ContactPhone,
//                IsForeign = s.IsForeign,
//                ResumeId = s.Resume != null ? s.Resume.Id : null,
//                StudyPlanId = s.StudyPlan.Id
//            })
//            .ToListAsync();
//    }

//    [HttpGet("{id}")]
//    public async Task<ActionResult<StudentDto>> GetStudentById(Guid id)
//    {
//        var student = await _context.Students
//            .Include(s => s.Resume)
//            .Include(s => s.StudyPlan)
//            .FirstOrDefaultAsync(s => s.Id == id);
        
//        if (student == null)
//            return NotFound();

//        return Ok(new StudentDto
//        {
//            Id = student.Id,
//            FirstName = student.FirstName,
//            LastName = student.LastName,
//            Email = student.Email,
//            Gender = student.Gender.GetDisplayName(),
//            BirthDate = student.BirthDate,
//            Photo = student.Photo,
//            ContactPhone = student.ContactPhone,
//            IsForeign = student.IsForeign,
//            ResumeId = student.Resume != null ? student.Resume.Id : null,
//            StudyPlanId = student.StudyPlan.Id
//        });
//    }

//    [HttpPost]
//    public async Task<ActionResult<StudentDto>> CreateStudent(StudentDto studentDto)
//    {
//        var studyPlan = new StudyPlan
//        {
//            Id = Guid.NewGuid(),
//            StudentId = Guid.NewGuid(),
//            CourseNumber = studyPlanDto.CourseNumber,
//            FacultyId = Guid.NewGuid(),
//            SpecialityId = Guid.NewGuid(),
//            StudyForm = EnumExtensions.FromDisplayName<StudyPlan.StudyForms>(studyPlanDto.StudyForm),
//            BeginYear = DateTime.UtcNow
//        };

//        var student = new Student
//        {
//            Id = Guid.NewGuid(),
//            Email = studentDto.Email,
//            PasswordHash = "temporary_password_hash",
//            CreatedAt = DateTime.UtcNow,
//            FirstName = studentDto.FirstName,
//            LastName = studentDto.LastName,
//            Gender = EnumExtensions.FromDisplayName<Student.StudentGender>(studentDto.Gender),
//            BirthDate = studentDto.BirthDate,
//            Photo = studentDto.Photo,
//            ContactPhone = studentDto.ContactPhone,
//            IsForeign = studentDto.IsForeign,
//            StudyPlan = studyPlan
//        };

//        studyPlan.StudentId = student.Id;

//        _context.StudyPlans.Add(studyPlan);
//        _context.Students.Add(student);
//        await _context.SaveChangesAsync();
//        studentDto.StudyPlanId = studyPlan.Id;
//        studentDto.Id = student.Id;

//        return CreatedAtAction(nameof(GetStudentById),
//            new { id = student.Id }, studentDto);
//    }
//}
