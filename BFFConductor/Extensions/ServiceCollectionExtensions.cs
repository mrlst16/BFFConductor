using System.Text.Json;
using BFFConductor.Configuration;
using BFFConductor.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BFFConductor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBFFConductor(
        this IServiceCollection services,
        Action<BffResponseOptions> configure)
    {
        var options = new BffResponseOptions();
        configure(options);

        var json = File.ReadAllText(options.MappingSpecPath);
        var config = JsonSerializer.Deserialize<BffMappingConfig>(json)
            ?? throw new InvalidOperationException($"Failed to deserialize mapping spec at '{options.MappingSpecPath}'.");

        var errorRegistry = ErrorMappingRegistry.LoadFrom(config, options.FallbackDisplayMode);
        var exceptionRegistry = ExceptionMappingRegistry.LoadFrom(config, options.FallbackDisplayMode);

        services.AddSingleton(options);
        services.AddSingleton(errorRegistry);
        services.AddSingleton(exceptionRegistry);
        services.AddTransient<BffExceptionFilter>();

        return services;
    }
}
