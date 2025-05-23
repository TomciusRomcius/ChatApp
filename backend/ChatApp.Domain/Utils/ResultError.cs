namespace ChatApp.Domain.Utils;

public enum ResultErrorType
{
    UNKNOWN_ERROR = 0,
    VALIDATION_ERROR,
    UNAUTHORIZED_ERROR,
    FORBIDDEN_ERROR,
    NOT_FOUND,
    ACCOUNT_SETUP_REQUIRED,
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