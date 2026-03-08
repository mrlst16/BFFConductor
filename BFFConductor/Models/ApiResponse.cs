using BFFConductor.Interfaces;

namespace BFFConductor.Models;

public class ApiResponse<T> : IApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<ApiError> Errors { get; init; } = new();

    IReadOnlyList<ApiError> IApiResponse<T>.Errors => Errors;
}

public class ApiError
{
    public string Message { get; init; } = string.Empty;
    public string? Code { get; init; }
}
