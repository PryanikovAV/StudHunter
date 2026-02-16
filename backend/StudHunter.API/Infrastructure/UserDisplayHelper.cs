using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Infrastructure;

public static class UserRoles
{
    public const string Student = nameof(DB.Postgres.Models.Student);
    public const string Employer = nameof(DB.Postgres.Models.Employer);
    public const string Administrator = nameof(DB.Postgres.Models.Administrator);

    public static string GetRole(User user) => user switch
    {
        DB.Postgres.Models.Student => Student,
        DB.Postgres.Models.Employer => Employer,
        DB.Postgres.Models.Administrator => Administrator,
        _ => nameof(User)
    };
}

public static class UserDefaultNames
{
    public const string DefaultFirstName = "Имя";
    public const string DefaultLastName = "Фамилия";
    public const string DefaultCompanyName = "Новая компания";
    public const string DefaultDeletedAccountName = "Удалённый аккаунт";
    public const string DefaultUserName = "Пользователь";
}

public static class UserDisplayHelper
{
    public static string GetUserDisplayName(User user) => user switch
    {
        Student s => 
        $"{s.LastName ?? UserDefaultNames.DefaultLastName} " +
        $"{s.FirstName ?? UserDefaultNames.DefaultFirstName} " +
        $"{s.Patronymic ?? ""}".Trim(),

        Employer e => e.Name ?? e.ContactEmail ?? UserDefaultNames.DefaultCompanyName,

        Administrator => UserRoles.Administrator,
        _ => user.Email ?? nameof(User)
    };

    public static int? CalculateAge(DateOnly? birthDate)
    {
        if (!birthDate.HasValue) return null;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Value.Year;
        if (birthDate.Value > today.AddYears(-age)) age--;
        return age;
    }
}