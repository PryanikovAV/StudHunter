namespace StudHunter.API.Common;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;

    public int ErrorCode { get; set; }
}