namespace StudHunter.DB.Postgres.Models;
// TODO: Добавить связь с городом, факультетами, кафедрами и специальностями
public class Course
{
    public Guid Id { get; init; }

    private string _name = null!;
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value.Trim();
        }
    }

    public string? Description { get; set; }

    public virtual ICollection<VacancyCourse> VacancyCourses { get; set; } = new HashSet<VacancyCourse>();
    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new HashSet<StudyPlanCourse>();
}
