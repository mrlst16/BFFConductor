using System.Text.Json.Serialization;

namespace BFFConductor.Configuration;

internal class BffMappingConfig
{
    [JsonPropertyName("defaults")]
    public BffMappingDefaults? Defaults { get; init; }

    [JsonPropertyName("mappings")]
    public List<BffMappingEntry> Mappings { get; init; } = new();

    [JsonPropertyName("exceptionMappings")]
    public List<BffExceptionMappingEntry> ExceptionMappings { get; init; } = new();
}

internal class BffMappingDefaults
{
    [JsonPropertyName("displayMode")]
    public string DisplayMode { get; init; } = string.Empty;
}

internal class BffMappingEntry
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; init; } = string.Empty;

    [JsonPropertyName("httpStatus")]
    public int HttpStatus { get; init; }

    [JsonPropertyName("displayMode")]
    public string DisplayMode { get; init; } = string.Empty;

    [JsonPropertyName("additionalHeaders")]
    public Dictionary<string, string>? AdditionalHeaders { get; init; }
}

internal class BffExceptionMappingEntry
{
    [JsonPropertyName("exceptionType")]
    public string ExceptionType { get; init; } = string.Empty;

    [JsonPropertyName("httpStatus")]
    public int HttpStatus { get; init; }

    [JsonPropertyName("displayMode")]
    public string DisplayMode { get; init; } = string.Empty;

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; init; }

    [JsonPropertyName("additionalHeaders")]
    public Dictionary<string, string>? AdditionalHeaders { get; init; }
}
