namespace StudHunter.API.ModelsDto;

public record LookupDto(Guid Id, string Name);
public record StudyDirectionDto(Guid Id, string Name, string? Code);
public record CourseDto(Guid Id, string Name, string? Description);
public record DepartmentDto(Guid Id, string Name, string? Description);
public record UniversityDto(Guid Id, string Name, string? Abbreviation);
public record CreateSkillDto(string Name);
public record UpdateSkillDto(string Name);