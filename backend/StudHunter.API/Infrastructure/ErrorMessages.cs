namespace StudHunter.API.Infrastructure;

public static class ErrorMessages
{
    public static string EntityNotFound(string entityName) => $"{entityName} not found.";
    public static string AlreadyExists(string entityName, string entityField = "")
    {
        if (!string.IsNullOrEmpty(entityField))
        {
            return $"{entityName} with the same {entityField} already exists.";
        }
        return $"{entityName} already exists.";
    }
    public static string OperationNotAllowed(string entityName, string operation = "")
    {
        if (!string.IsNullOrEmpty(operation))
        {
            return $"Operation '{operation}' is not allowed on {entityName}.";
        }
        return $"Operation is not allowed on {entityName}.";
    }
    public static string InvalidData(string entityName) => $"Invalid {entityName} data.";
    public static string InvalidCredentials() => "Invalid email or password.";
    public static string AccountAlreadyActive() => "Account is already active.";
    public static string SelfActionNotAllowed() => "You cannot perform this action on yourself.";
    public static string RestrictProfileActions(string action, string field) => $"You can only {action} your own {field}.";
    public static string RecoveryFailed() => "Failed to recover account.";
    public static string AdminSelfRecovery() => "Administrators cannot restore accounts themselves.";
    public static string FailedToDelete(string entityName) => $"Failed to delete {entityName}.";
    public static string EntityAlreadyDeleted(string entityName) => $"{entityName} is already deleted.";
    public static string CommunicationBlocked() => "Communication is blocked by one of the users.";
    public static string CompleteProfileFirst() => "Please complete your profile to access this feature.";
    public static string IncorrectPassword() => "Incorrect password.";


    /* Неиспользуемые мои старые */

    public static string CannotDelete(string entityName, string field) => $"Cannot delete {entityName} associated with {field}.";
    public static string InvalidTokenUserId() => "Invalid user ID in token.";
    public static string FailedToRetrieve(string entityName) => $"Failed to retrieve {entityName} from database.";
    public static string FailedToSave(string entityName) => $"Failed to save {entityName} to database.";

    /* Нейронка, неиспользуемые */
    // Ошибки доступа и разрешений
    public static string AccessDenied() => "You do not have permission to perform this action.";
    public static string Unauthorized() => "User is not authorized.";
    public static string ForbiddenAction() => "This action is not allowed for your role.";

    // Ошибки коммуникации (ЧС и Роли)

    public static string SelfActionNotAllowed(string operation) => $"You cannot {operation} this action on yourself.";
    public static string InvalidRoleInteraction() => "Action not allowed between users of the same role.";

    // Ошибки базы данных и системы
    public static string DatabaseError() => "A database error occurred while saving changes.";
    public static string UnexpectedError() => "An unexpected error occurred. Please try again later.";
    public static string AccountRecoveryFailed() => "Failed to recover account.";
}
