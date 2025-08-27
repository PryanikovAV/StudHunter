using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;

namespace StudHunter.API.Common;

public static class ErrorMessages
{
    private static string FormatErrorMessage(string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
            return errorMessage;
        return char.ToUpper(errorMessage[0]) + errorMessage[1..].ToLower();
    }

    /// <summary>
    /// Entity not found in the database.
    /// </summary>
    public static string EntityNotFound(string entityName) => FormatErrorMessage($"{entityName} not found.");

    /// <summary>
    /// Invalid data provided for the specified entity or field.
    /// </summary>
    public static string InvalidData(string entityName) => FormatErrorMessage($"Invalid {entityName} data.");

    /// <summary>
    /// Entity with the specified field value already exists.
    /// </summary>
    public static string EntityAlreadyExists(string entityName, string field) => FormatErrorMessage($"{entityName} with this {field} already exists.");

    /// <summary>
    /// Entity is already deleted. Use method to restore.
    /// </summary>
    public static string EntityAlreadyDeleted(string entityName, string methodName) => FormatErrorMessage($"{entityName} is already deleted. Use {methodName} to restore.");

    /// <summary>
    /// Cannot delete entity due to associations with another entity or field.
    /// </summary>
    public static string CannotDelete(string entityName, string field) => FormatErrorMessage($"Cannot delete {entityName} associated with {field}.");

    /// <summary>
    /// Invalid user ID in the authentication token.
    /// </summary>
    public static string InvalidTokenUserId() => FormatErrorMessage("Invalid user ID in token.");

    /// <summary>
    /// User can only perform the specified action on their own profile or field.
    /// </summary>
    public static string RestrictOwnProfileAction(string action, string field) => FormatErrorMessage($"You can only {action} your own {field}.");

    /// <summary>
    /// Failed to retrieve the entity from the database after an operation.
    /// </summary>
    public static string FailedToRetrieve(string entityName) => FormatErrorMessage($"Failed to retrieve {entityName} from database.");

    /// <summary>
    /// Failed to save changes for the entity to the database.
    /// </summary>
    public static string FailedToSave(string entityName) => FormatErrorMessage($"Failed to save {entityName} to database.");

    /// <summary>
    /// Operation is not allowed for the entity due to its current state.
    /// </summary>
    public static string OperationNotAllowed(string entityName, string operation) => FormatErrorMessage($"Operation '{operation}' not allowed for {entityName}.");
}
