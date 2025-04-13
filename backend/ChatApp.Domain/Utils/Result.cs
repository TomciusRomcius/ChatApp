namespace ChatApp.Domain.Utils;

public class Result<T>
{
    public Result(T? returnValue, IEnumerable<ResultError> errors)
    {
        ReturnValue = returnValue;
        Errors = errors;
    }

    public Result(T? returnValue)
    {
        ReturnValue = returnValue;
        Errors = [];
    }

    public Result(IEnumerable<ResultError> errors)
    {
        Errors = errors;
    }

    public T? ReturnValue { get; init; }
    public IEnumerable<ResultError> Errors { get; init; }

    public bool IsError()
    {
        return Errors.Any();
    }

    /// <summary>
    ///     Returns the value but throws an exception if used when Error is set
    /// </summary>
    public T GetValue()
    {
        if (Errors.Any()) throw new InvalidOperationException("Trying to get a result value when an error is set!");

        return ReturnValue!;
    }
}