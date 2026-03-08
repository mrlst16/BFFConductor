using System.Text.Json;
using System.Text.Json.Serialization;

namespace BFFConductor.Configuration;

public class ErrorMappingRegistry
{
    private readonly Dictionary<string, ErrorMapping> _mappings;

    public string DefaultDisplayMethod { get; }

    private ErrorMappingRegistry(Dictionary<string, ErrorMapping> mappings, string defaultDisplayMethod)
    {
        _mappings = mappings;
        DefaultDisplayMethod = defaultDisplayMethod;
    }

    public static ErrorMappingRegistry LoadFrom(string path, string fallbackDisplayMethod)
    {
        var json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<BffMappingConfig>(json)
            ?? throw new InvalidOperationException($"Failed to deserialize mapping spec at '{path}'.");

        var defaultDisplayMethod = config.Defaults?.DisplayMethod ?? fallbackDisplayMethod;

        var dict = config.Mappings.ToDictionary(
            m => m.ErrorCode,
            m => new ErrorMapping
            {
                ErrorCode = m.ErrorCode,
                HttpStatus = m.HttpStatus,
                DisplayMethod = m.DisplayMethod,
                AdditionalHeaders = m.AdditionalHeaders
            });

        return new ErrorMappingRegistry(dict, defaultDisplayMethod);
    }

    /// <summary>Returns a cloned dictionary so callers can safely mutate it for per-request resolution.</summary>
    public Dictionary<string, ErrorMapping> CloneMappings() => new(_mappings);
}

file class BffMappingConfig
{
    [JsonPropertyName("defaults")]
    public BffMappingDefaults? Defaults { get; init; }

    [JsonPropertyName("mappings")]
    public List<BffMappingEntry> Mappings { get; init; } = new();
}

file class BffMappingDefaults
{
    [JsonPropertyName("displayMethod")]
    public string DisplayMethod { get; init; } = string.Empty;
}

file class BffMappingEntry
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; init; } = string.Empty;

    [JsonPropertyName("httpStatus")]
    public int HttpStatus { get; init; }

    [JsonPropertyName("displayMethod")]
    public string DisplayMethod { get; init; } = string.Empty;

    [JsonPropertyName("additionalHeaders")]
    public Dictionary<string, string>? AdditionalHeaders { get; init; }
}
