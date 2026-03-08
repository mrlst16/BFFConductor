using BFFConductor.Models;

namespace BFFConductor.Interfaces;

public interface IApiResponse<out T>
{
    bool Success { get; }
    T? Data { get; }
    IReadOnlyList<ApiError> Errors { get; }
}
