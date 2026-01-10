using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Infrastructure;

public enum UserAction
{
    CreateVacancy,
    ViewVacancies,

    CreateResume,
    ViewResumes,

    SendInvitation,
    AcceptInvitation,
    AddToFavorite,
    ViewContacts,

    SendMessage,
    ViewChats
}

public static class UserPermissions
{
    private static readonly Dictionary<(string Role, User.AccountStatus Stage), HashSet<UserAction>> _registry = new()
    {
        [(UserRoles.Student, User.AccountStatus.Anonymous)] = new()
        {
            UserAction.ViewVacancies,
            UserAction.AddToFavorite
        },
        [(UserRoles.Student, User.AccountStatus.ProfileFilled)] = new()
        {
            UserAction.ViewVacancies,
            UserAction.AddToFavorite,
            UserAction.CreateResume,
            UserAction.SendInvitation,
            UserAction.AcceptInvitation,
            UserAction.SendMessage,
            UserAction.ViewChats
        },
        [(UserRoles.Employer, User.AccountStatus.Anonymous)] = new()
        {
            UserAction.ViewResumes
        },
        [(UserRoles.Employer, User.AccountStatus.ProfileFilled)] = new()
        {
            UserAction.ViewResumes,
            UserAction.AddToFavorite,
            UserAction.CreateVacancy
        }
    };

    public static bool IsAllowed(string role, User.AccountStatus stage, UserAction action)
    {
        if (stage == User.AccountStatus.FullyActivated)
            return true;

        return _registry.TryGetValue((role, stage), out var actions) && actions.Contains(action);
    }

    public static string GetPermissionErrorMessage(string role, User.AccountStatus stage)
    {
        return (role, stage) switch
        {
            (UserRoles.Employer, User.AccountStatus.Anonymous) =>
                "Действие заблокировано. Ваша организация ожидает аккредитации администратором.",
            (UserRoles.Employer, User.AccountStatus.ProfileFilled) =>
                "Необходимо исправить ошибки в профиле или дождаться подтверждения данных.",
            (UserRoles.Student, User.AccountStatus.Anonymous) =>
                "Пожалуйста, заполните данные профиля, чтобы получить доступ к этой функции.",
            _ => "Ваш текущий статус профиля не позволяет выполнить это действие."
        };
    }
}