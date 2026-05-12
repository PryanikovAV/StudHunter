namespace StudHunter.API.Infrastructure;

public static class ErrorMessages
{
    // Базовые ошибки
    public static string EntityNotFound(string entityName) => $"{entityName} не найден(а).";
    public static string InvalidData(string entityName) => $"Некорректные данные для {entityName}.";

    // Ошибки конфликтов
    public static string AlreadyExists(string entityName, string entityField = "")
    {
        if (!string.IsNullOrEmpty(entityField))
            return $"{entityName} с таким {entityField} уже существует.";

        return $"{entityName} уже существует.";
    }

    // Авторизация и аккаунт
    public static string InvalidCredentials() => "Неверный email или пароль.";
    public static string IncorrectPassword() => "Неверный текущий пароль.";
    public static string AccountAlreadyActive() => "Аккаунт уже активирован.";
    public static string CompleteProfileFirst() => "Пожалуйста, заполните данные профиля для доступа к этой функции.";

    // Восстановление
    public static string RecoveryFailed() => "Не удалось восстановить аккаунт.";
    public static string AdminSelfRecovery() => "Администратор не может восстанавливать аккаунты самостоятельно.";

    // Ограничения бизнес-логики
    public static string CommunicationBlocked() => "Взаимодействие невозможно. Пользователь находится в черном списке.";
    public static string SelfActionNotAllowed() => "Вы не можете выполнить это действие по отношению к самому себе.";
    public static string RestrictProfileActions(string action, string field) => $"Вы можете {action} только свой собственный {field}.";

    // Удаление
    public static string FailedToDelete(string entityName) => $"Не удалось удалить {entityName}.";
    public static string EntityAlreadyDeleted(string entityName) => $"{entityName} уже удален(а).";

    // Прочее
    public static string OperationNotAllowed(string entityName, string operation = "")
    {
        if (!string.IsNullOrEmpty(operation))
            return $"Операция '{operation}' запрещена для {entityName}.";

        return $"Операция запрещена для {entityName}.";
    }
}