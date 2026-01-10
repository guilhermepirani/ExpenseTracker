namespace AppCore;

/// <summary>
/// Base Result class for representing success or failure states
/// </summary>
public abstract class ResultBase
{
    public bool IsSuccess { get; protected set; }
    public string[]? Errors { get; protected set; }

    /// <summary>
    /// Creates a failure result instance with the given errors
    /// </summary>
    public abstract ResultBase CreateFailureInstance(string[] errors);
}

/// <summary>
/// Generic Result class that includes a success value of type T
/// </summary>
public class Result<T> : ResultBase
{
    public T? Data { get; private set; }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Data = value, Errors = null };

    public static Result<T> Failure(params string[] errors) =>
        new() { IsSuccess = false, Errors = errors, Data = default };

    public static Result<T> FromException(Exception ex) =>
        Failure(ex.Message);

    public override ResultBase CreateFailureInstance(string[] errors) =>
        Failure(errors);
}

/// <summary>
/// Non-generic Result for void operations
/// </summary>
public class Result : ResultBase
{
    public static Result Success() =>
        new() { IsSuccess = true, Errors = null };

    public static Result Failure(params string[] errors) =>
        new() { IsSuccess = false, Errors = errors };

    public static Result FromException(Exception ex) =>
        Failure(ex.Message);

    public override ResultBase CreateFailureInstance(string[] errors) =>
        Failure(errors);
}

/// <summary>
/// Static factory for creating Result failures
/// </summary>
public static class ResultFactory
{
    public static TResult CreateFailure<TResult>(string[] errors)
        where TResult : ResultBase, new()
    {
        var instance = new TResult();
        return (TResult)instance.CreateFailureInstance(errors);
    }
}