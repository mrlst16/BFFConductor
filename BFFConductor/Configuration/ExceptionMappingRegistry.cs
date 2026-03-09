namespace BFFConductor.Configuration;

public class ExceptionMappingRegistry
{
    private readonly List<ExceptionMapping> _mappings;

    public string DefaultDisplayMode { get; }

    private ExceptionMappingRegistry(List<ExceptionMapping> mappings, string defaultDisplayMode)
    {
        _mappings = mappings;
        DefaultDisplayMode = defaultDisplayMode;
    }

    internal static ExceptionMappingRegistry LoadFrom(BffMappingConfig config, string fallbackDisplayMode)
    {
        var defaultDisplayMode = config.Defaults?.DisplayMode ?? fallbackDisplayMode;

        var resolved = config.ExceptionMappings
            .Select(entry =>
            {
                var type = ResolveType(entry.ExceptionType)
                    ?? throw new InvalidOperationException(
                        $"ExceptionMappingRegistry: could not resolve exception type '{entry.ExceptionType}' " +
                        $"from any loaded assembly. Ensure the assembly is referenced.");

                return new ExceptionMapping
                {
                    ExceptionTypeName = entry.ExceptionType,
                    ResolvedType = type,
                    HttpStatus = entry.HttpStatus,
                    DisplayMode = entry.DisplayMode,
                    ErrorCode = entry.ErrorCode,
                    AdditionalHeaders = entry.AdditionalHeaders
                };
            })
            .ToList();

        resolved.Sort(CompareBySpecificity);

        return new ExceptionMappingRegistry(resolved, defaultDisplayMode);
    }

    /// <summary>Returns a cloned list (most-derived-first) so callers can safely apply per-request overrides.</summary>
    public List<ExceptionMapping> CloneMappings() => new(_mappings);

    /// <summary>Finds the most specific mapping for the given exception type.</summary>
    public static ExceptionMapping? FindMapping(IList<ExceptionMapping> orderedMappings, Type exceptionType) =>
        orderedMappings.FirstOrDefault(m => m.ResolvedType!.IsAssignableFrom(exceptionType));

    private static Type? ResolveType(string simpleTypeName) =>
        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); } catch { return []; } })
            .FirstOrDefault(t => t.Name == simpleTypeName && typeof(Exception).IsAssignableFrom(t));

    private static int CompareBySpecificity(ExceptionMapping a, ExceptionMapping b)
    {
        if (a.ResolvedType == b.ResolvedType) return 0;
        if (a.ResolvedType!.IsAssignableFrom(b.ResolvedType)) return 1;  // b is more derived → b first
        if (b.ResolvedType!.IsAssignableFrom(a.ResolvedType)) return -1; // a is more derived → a first
        return 0;
    }
}
