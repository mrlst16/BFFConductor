namespace BFFConductor.Configuration;

public class ErrorMappingRegistry
{
    private readonly Dictionary<string, ErrorMapping> _mappings;

    public string DefaultDisplayMode { get; }

    private ErrorMappingRegistry(Dictionary<string, ErrorMapping> mappings, string defaultDisplayMode)
    {
        _mappings = mappings;
        DefaultDisplayMode = defaultDisplayMode;
    }

    internal static ErrorMappingRegistry LoadFrom(BffMappingConfig config, string fallbackDisplayMode)
    {
        var defaultDisplayMode = config.Defaults?.DisplayMode ?? fallbackDisplayMode;

        var dict = config.Mappings.ToDictionary(
            m => m.ErrorCode,
            m => new ErrorMapping
            {
                ErrorCode = m.ErrorCode,
                HttpStatus = m.HttpStatus,
                DisplayMode = m.DisplayMode,
                AdditionalHeaders = m.AdditionalHeaders
            });

        return new ErrorMappingRegistry(dict, defaultDisplayMode);
    }

    /// <summary>Returns a cloned dictionary so callers can safely mutate it for per-request resolution.</summary>
    public Dictionary<string, ErrorMapping> CloneMappings() => new(_mappings);
}
