namespace StudHunter.API.ModelsDto.Student;

public class AdminUpdateStudentDto : BaseUpdateStudentDto
{
    public bool? IsDeleted { get; set; }
}
