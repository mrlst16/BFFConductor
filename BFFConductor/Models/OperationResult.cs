using BFFConductor.Interfaces;

namespace BFFConductor.Models;

public class OperationResult<T> : IOperationResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<OperationError> Errors { get; init; } = new();
    public string? DisplayMethod { get; init; }

    IReadOnlyList<OperationError> IOperationResult.Errors => Errors;
    object? IOperationResult.GetData() => Data;

    public static OperationResult<T> Ok(T data, string? displayMethod = null) =>
        new() { Success = true, Data = data, DisplayMethod = displayMethod };

    public static OperationResult<T> Fail(string message, string? code = null) =>
        new() { Success = false, Errors = [new OperationError { Message = message, Code = code }] };

    public static OperationResult<T> Fail(IEnumerable<OperationError> errors) =>
        new() { Success = false, Errors = errors.ToList() };
}

public class OperationError
{
    public string Message { get; init; } = string.Empty;
    public string? Code { get; init; }
}
