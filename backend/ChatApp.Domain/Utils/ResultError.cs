namespace ChatApp.Domain.Utils;

public enum ResultErrorType
{
    VALIDATION_ERROR,
    FORBIDDEN_ERROR,
    NOT_FOUND
}

public class ResultError
{
    public ResultError(ResultErrorType type, string message)
    {
        Type = type;
        Message = message;
    }

    public ResultErrorType Type { get; init; }
    public string Message { get; init; }
}