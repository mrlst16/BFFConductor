namespace BFFConductor.Configuration;

public record ExceptionMapping
{
    public string ExceptionTypeName { get; init; } = string.Empty;
    public Type? ResolvedType { get; init; }
    public int HttpStatus { get; init; }
    public string DisplayMode { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
    public Dictionary<string, string>? AdditionalHeaders { get; init; }
}
