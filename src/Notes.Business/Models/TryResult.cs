public readonly struct TryResult<T> 
    where T : class
{
    public TryResult(T data)
    {
        Data = data;
    }

    public TryResult(int statusCode, string field, string message)
    {
        StatusCode = statusCode;
        Data = null;
        Errors[field] = [message];
    }

    public TryResult(int statusCode)
    {
        StatusCode = statusCode;
    }

    public TryResult(ResultErrors errors)
    {
        StatusCode = 400;
        Errors = errors;
    }

    public T? Data { get; } = null;

    public ResultErrors Errors { get; } = new();
    public int StatusCode { get; } = 200;

    public void Deconstruct(out bool success, out ResultErrors errors, out int statusCode, out T? data)
    {
        success = !Errors.Any() && StatusCode == 200;
        errors = Errors;
        statusCode = StatusCode;
        data = Data;
    }

    public static TryResult<T> Succeed(T data) => new (data);

    public static TryResult<T> BadRequest(string field, string message) => new(400, field, message);
    public static TryResult<T> Forbidden(string field, string message) => new(403, field, message);
    public static TryResult<T> NotFound() => new(404);
    public static TryResult<T> Conflict(string field, string message) => new(409, field, message);
    public static TryResult<T> Gone() => new(410);
    public static TryResult<T> Unprocessable(string field, string message) => new(422, field, message);

    public static TryResult<T> Loop(string field, string message) => new(508, field, message);
}

/// <inheritdoc/>
public class ResultErrors : Dictionary<string, HashSet<string>>
{
    public ResultErrors() : base() 
    { 
    }

    public ResultErrors(IDictionary<string, HashSet<string>> dictionary) : base(dictionary)
    {
    }
}