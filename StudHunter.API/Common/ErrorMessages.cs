namespace StudHunter.API.Common;

/// <summary>
/// Provides standardized error messages for API responses.
/// </summary>
public static class ErrorMessages
{
    /// <summary>
    /// Formats a string so that the first letter is uppercase and all other letters are lowercase. 
    /// </summary>
    /// <param name="errorMessage">The original string.</param>
    /// <returns>The formatted string.</returns>
    private static string FormatErrorMessage(string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
            return errorMessage;
        return char.ToUpper(errorMessage[0]) + errorMessage[1..].ToLower();
    }

    /// <summary>
    /// Returns an error message for a not found entity.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <returns>The error message in the format "[Entity] not found".</returns>
    public static string NotFound(string entityName) => FormatErrorMessage($"{entityName} not found.");

    /// <summary>
    /// Returns an error message for invalid data.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <returns>The error message in the format "Invalid [entity] data".</returns>
    public static string InvalidData(string entityName) => FormatErrorMessage($"invalid {entityName} data.");

    /// <summary>
    /// Returns an error message for an existing entity.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="field">The field causing the conflict.</param>
    /// <returns>The error message in the format "[Entity] with this [field] already exists".</returns>
    public static string AlreadyExists(string entityName, string field) => FormatErrorMessage($"{entityName} with this {field} already exists.");

    /// <summary>
    /// Returns an error message for an already deleted entity.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <returns>The error message in the format "[Entity] is already deleted".</returns>
    public static string AlreadyDeleted(string entityName) => FormatErrorMessage($"{entityName} is already deleted.");

    /// <summary>
    /// Returns an error message for an entity that cannot be deleted due to associated records.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="field">The associated records preventing deletion.</param>
    /// <returns>The error message in the format "cannot delete [entity] associated with [field]".</returns>
    public static string CannotDelete(string entityName, string field) => FormatErrorMessage($"cannot delete {entityName} associated with {field}.");
}
