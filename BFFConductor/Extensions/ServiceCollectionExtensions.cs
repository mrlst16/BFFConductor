using BFFConductor.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BFFConductor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBffResponse(
        this IServiceCollection services,
        Action<BffResponseOptions> configure)
    {
        var options = new BffResponseOptions();
        configure(options);

        var registry = ErrorMappingRegistry.LoadFrom(options.MappingSpecPath, options.FallbackDisplayMethod);

        services.AddSingleton(options);
        services.AddSingleton(registry);

        return services;
    }
}
