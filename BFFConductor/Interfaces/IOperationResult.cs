using BFFConductor.Models;

namespace BFFConductor.Interfaces;

public interface IOperationResult
{
    bool Success { get; }
    IReadOnlyList<OperationError> Errors { get; }
    string? DisplayMode { get; }
    object? GetData();
}

public interface IOperationResult<out T> : IOperationResult
{
    T? Data { get; }
}
