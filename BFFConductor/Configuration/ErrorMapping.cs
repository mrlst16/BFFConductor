namespace BFFConductor.Configuration;

public record ErrorMapping
{
    public string ErrorCode { get; init; } = string.Empty;
    public int HttpStatus { get; init; }
    public string DisplayMethod { get; init; } = string.Empty;
    public Dictionary<string, string>? AdditionalHeaders { get; init; }
}
