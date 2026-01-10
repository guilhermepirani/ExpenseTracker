namespace AppCore;

/// <summary>
/// Base Result class for representing success or failure states
/// </summary>
public abstract class ResultBase
{
    public bool IsSuccess { get; protected set; }
    public string[]? Errors { get; protected set; }
    public int StatusCode { get; set; }

    /// <summary>
    /// Creates a failure result instance with the given errors
    /// </summary>
    public abstract ResultBase CreateFailureInstance(
        int statusCode, string[] errors);
}

/// <summary>
/// Generic Result class that includes a success value of string T
/// </summary>
public class Result<T> : ResultBase
{
    public T? Data { get; private set; }

    public static Result<T> Success(
        int statusCode, T value)
    {
        return new()
        {
            IsSuccess = true,
            Data = value,
            Errors = null,
            StatusCode = statusCode
        };
    }

    public static Result<T> Failure(
        int statusCode, params string[] errors)
    {
        return new()
        {
            IsSuccess = false,
            Errors = errors,
            Data = default,
            StatusCode = statusCode
        };
    }

    public static Result<T> FromException(
        int statusCode, Exception ex)
    {
        return Failure(statusCode, ex.Message);
    }

    public override ResultBase CreateFailureInstance(
        int statusCode, string[] errors)
    {
        return Failure(statusCode, errors);
    }
}

/// <summary>
/// Non-generic Result for void operations
/// </summary>
public class Result : ResultBase
{
    public static Result Success()
    {
        return new()
        {
            IsSuccess = true,
            Errors = null
        };
    }

    public static Result Failure(
        int statusCode, params string[] errors)
    {
        return new()
        {
            IsSuccess = false,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static Result FromException(
        int statusCode, Exception ex)
    {
        return Failure(statusCode, ex.Message);
    }

    public override ResultBase CreateFailureInstance(
        int statusCode, string[] errors)
    {
        return Failure(statusCode, errors);
    }
}

/// <summary>
/// Static factory for creating Result failures
/// </summary>
public static class ResultFactory
{
    public static TResult CreateFailure<TResult>(
        int statusCode, string[] errors)
        where TResult : ResultBase, new()
    {
        var instance = new TResult();
        return (TResult)instance
            .CreateFailureInstance(statusCode, errors);
    }
}