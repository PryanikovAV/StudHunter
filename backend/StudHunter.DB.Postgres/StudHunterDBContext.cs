using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres.Models;
using StudHunter.DB.Postgres.Configurations;

namespace StudHunter.DB.Postgres;

public class StudHunterDbContext : DbContext
{
    public StudHunterDbContext(DbContextOptions<StudHunterDbContext> options)
        : base(options)
    {
    }

    public DbSet<AdditionalSkill> AdditionalSkills => Set<AdditionalSkill>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<BlackList> BlackLists => Set<BlackList>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employer> Employers => Set<Employer>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OrganizationDetail> OrganizationDetails => Set<OrganizationDetail>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<ResumeAdditionalSkill> ResumeAdditionalSkills => Set<ResumeAdditionalSkill>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudyDirection> StudyDirections => Set<StudyDirection>();
    public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();
    public DbSet<StudyPlanCourse> StudyPlanCourses => Set<StudyPlanCourse>();
    public DbSet<University> Universities => Set<University>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
    public DbSet<VacancyAdditionalSkill> VacancyAdditionalSkills => Set<VacancyAdditionalSkill>();
    public DbSet<VacancyCourse> VacancyCourses => Set<VacancyCourse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().UseTpcMappingStrategy();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudHunterDbContext).Assembly);
    }
}
