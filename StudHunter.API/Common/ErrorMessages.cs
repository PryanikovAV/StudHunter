using System.Globalization;

namespace StudHunter.API.Common;

public static class ErrorMessages
{
    private static readonly TextInfo _textInfo = CultureInfo.InvariantCulture.TextInfo;

    public static string NotFound(string entityName) => $"{_textInfo.ToTitleCase(entityName)} not found";

    public static string InvalidData(string entityName) => $"Invalid {entityName.ToLower()} data";

    public static string AlreadyExists(string entityName, string field) => $"{_textInfo.ToTitleCase(entityName)} with this {field.ToLower()} already exists";

    public static string AlreadyDeleted(string entityName) => $"{_textInfo.ToTitleCase(entityName)} is already deleted";
}
