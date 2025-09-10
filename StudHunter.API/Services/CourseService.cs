using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing courses.
/// </summary>
public class CourseService(StudHunterDbContext context) : BaseCourseService(context)
{

}
