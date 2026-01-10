namespace StudHunter.API.ModelsDto;

public record LookupDto(Guid Id, string Name);
public record SpecialityLookupDto(Guid Id, string Name, string? Code);
public record CourseLookupDto(Guid Id, string Name, string? Description);
public record SkillDto(Guid Id, string Name);
public record CreateSkillDto(string Name);
public record UpdateSkillDto(string Name);