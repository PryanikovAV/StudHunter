using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres.Models;
using StudHunter.DB.Postgres.Configurations;

namespace StudHunter.DB.Postgres;

public class StudHunterDbContext : DbContext
{
    public DbSet<AchievementTemplate> AchievementTemplates { get; set; } = null!;
    public DbSet<Administrator> Administrators { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Employer> Employers { get; set; } = null!;
    public DbSet<Faculty> Faculties { get; set; } = null!;
    public DbSet<Favorite> Favorites { get; set; } = null!;
    public DbSet<Invitation> Invitations { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Resume> Resumes { get; set; } = null!;
    public DbSet<Speciality> Specialities { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<StudentStatus> StudentStatuses { get; set; } = null!;
    public DbSet<StudyPlan> StudyPlans { get; set; } = null!;
    public DbSet<StudyPlanCourse> StudyPlanCourses { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAchievement> UserAchievements { get; set; } = null!;
    public DbSet<Vacancy> Vacancies { get; set; } = null!;
    public DbSet<VacancyCourse> VacancyCourses { get; set; } = null!;

    public StudHunterDbContext(DbContextOptions<StudHunterDbContext> options)
        : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("studhunter");
        modelBuilder.Entity<User>().UseTpcMappingStrategy();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
