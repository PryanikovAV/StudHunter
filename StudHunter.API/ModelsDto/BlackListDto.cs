using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto;

public record BlackListDto(
    Guid Id,
    Guid BlockedUserId,
    string DisplayName,
    string? AvatarUrl,
    string Role,
    DateTime BlockedAt
);

public static class BlackListMapper
{
    public static BlackListDto ToDto(BlackList blackList)
    {
        var target = blackList.BlockedUser;
        return new BlackListDto(
            blackList.Id,
            blackList.BlockedUserId,
            target switch
            {
                Student s => $"{s.LastName} {s.FirstName}".Trim(),
                Employer e => e.Name,
                _ => target.Email
            },
            target.AvatarUrl,
            target switch
            {
                Student => nameof(Student),
                Employer => nameof(Employer),
                _ => nameof(User)
            },
            blackList.BlockedAt
        );
    }
}
