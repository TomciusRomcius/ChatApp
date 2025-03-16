namespace ChatApp.Server.Domain.Utils
{
    public enum ResultErrorType
    {
        VALIDATION_ERROR,
        FORBIDDEN_ERROR,
    }

    public class ResultError
    {
        public ResultErrorType Type { get; init; }
        public string Message { get; init; }

        public ResultError(ResultErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}