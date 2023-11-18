using static System.Runtime.InteropServices.JavaScript.JSType;

public readonly struct TryResult<T> 
    where T : class
{
    public TryResult(T data)
    {
        Data = data;
    }

    public TryResult(string field, string message)
    {
        Data = null;
        Errors[field] = [message];
    }

    public bool Success => !Errors.Any();

    public T? Data { get; } = null;

    public ResultErrors Errors { get; } = new();

    public void Deconstruct(out bool success, out ResultErrors errors, out T? data)
    {
        success = Success;
        errors = Errors;
        data = Data;
    }
    public static TryResult<T> Succeed(T data) => new (data);

    public static TryResult<T> Error(string field, string message) => new(field, message);
}

/// <inheritdoc/>
public class ResultErrors : Dictionary<string, HashSet<string>> { }