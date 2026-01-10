namespace StudHunter.API.Infrastructure;

public record Result<T>(
    T? Value,
    bool IsSuccess,
    string? ErrorMessage = null,
    int StatusCode = StatusCodes.Status200OK)
{
    // GET, CREATE
    public static Result<T> Success(T value) => new(value, true);

    // ERRORS
    public static Result<T> Failure(string errorMessage, int statusCode = StatusCodes.Status400BadRequest)
        => new(default, false, errorMessage, statusCode);

    // Result<bool>, нет возвращаемых данных
    public static Result<bool> Success() => new(true, true, null, StatusCodes.Status200OK);
}