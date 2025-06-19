//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using StudHunter.API.Extensions;
//using StudHunter.API.Models;
//using StudHunter.DB.Postgres;
//using StudHunter.DB.Postgres.Models;

//namespace StudHunter.API.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class StudentPlansController : ControllerBase
//{
//    private readonly StudHunterDbContext _context;

//    public StudentPlansController(StudHunterDbContext context)
//    {
//        _context = context;
//    }

//    [HttpGet("{id}")]
//    public async Task<ActionResult<StudyPlanDto>> GetStudyPlan(Guid id)
//    {
//        var studyPlan = await _context.StudyPlans
//            .Include(sp => sp.Faculty)
//            .Include(sp => sp.Speciality)
//            .Include(sp => sp.StudyPlanCourses)
//            .FirstOrDefaultAsync(sp => sp.Id == id);
        
//        if (studyPlan == null)
//            return NotFound();
        
//        return Ok(new StudyPlanDto
//        {
//            Id = studyPlan.Id,
//            StudentId = studyPlan.StudentId,
//            CourseNumber = studyPlan.CourseNumber,
//            FacultyId = studyPlan.FacultyId,
//            FacultyName = studyPlan.Faculty.Name,
//            SpecialityId = studyPlan.SpecialityId,
//            SpecialityName = studyPlan.Speciality.Name,
//            StudyForm = studyPlan.StudyForm.GetDisplayName(),
//            BeginYear = studyPlan.BeginYear,
//            StudyPlanCourseIds = studyPlan.StudyPlanCourses.Select(spc => spc.Id).ToList()
//        });
//    }
//}
