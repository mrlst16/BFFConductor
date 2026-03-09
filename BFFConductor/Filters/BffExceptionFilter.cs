using System.Reflection;
using BFFConductor.Attributes;
using BFFConductor.Configuration;
using BFFConductor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BFFConductor.Filters;

public class BffExceptionFilter : IExceptionFilter
{
    private const string HeaderName = "x-handle-message-as";

    private readonly ExceptionMappingRegistry _registry;
    private readonly BffResponseOptions _options;

    public BffExceptionFilter(ExceptionMappingRegistry registry, BffResponseOptions options)
    {
        _registry = registry;
        _options = options;
    }

    public void OnException(ExceptionContext context)
    {
        var orderedMappings = _registry.CloneMappings();

        // Controller-level [ExceptionDisplay] overrides
        var controllerType = context.ActionDescriptor is ControllerActionDescriptor cad1
            ? cad1.ControllerTypeInfo.AsType()
            : context.HttpContext.RequestServices.GetType();

        var controllerAttrs = controllerType.GetCustomAttributes<ExceptionDisplayAttribute>(inherit: true);
        foreach (var attr in controllerAttrs)
            ApplyOverride(attr, orderedMappings, controllerType.Name);

        // Action-level [ExceptionDisplay] overrides (most specific wins)
        if (context.ActionDescriptor is ControllerActionDescriptor cad2)
        {
            var actionAttrs = cad2.MethodInfo.GetCustomAttributes<ExceptionDisplayAttribute>(inherit: false);
            foreach (var attr in actionAttrs)
                ApplyOverride(attr, orderedMappings, cad2.MethodInfo.Name);
        }

        var mapping = ExceptionMappingRegistry.FindMapping(orderedMappings, context.Exception.GetType());
        if (mapping is null) return; // Unknown exception — let default handling proceed

        var displayMode = mapping.DisplayMode ?? _registry.DefaultDisplayMode;

        context.HttpContext.Response.Headers[HeaderName] = displayMode;

        if (mapping.AdditionalHeaders is not null)
        {
            foreach (var (key, value) in mapping.AdditionalHeaders)
                context.HttpContext.Response.Headers[key] = value;

            var currentExposed = context.HttpContext.Response.Headers["Access-Control-Expose-Headers"].ToString();
            var toAppend = string.Join(", ", mapping.AdditionalHeaders.Keys);
            context.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = string.IsNullOrEmpty(currentExposed)
                ? toAppend
                : currentExposed + ", " + toAppend;
        }

        context.Result = new ObjectResult(new ApiResponse<object?>
        {
            Success = false,
            Data = null,
            Errors = [new ApiError { Message = context.Exception.Message, Code = mapping.ErrorCode }]
        })
        { StatusCode = mapping.HttpStatus };

        context.ExceptionHandled = true;
    }

    private static void ApplyOverride(ExceptionDisplayAttribute attr, List<ExceptionMapping> orderedMappings, string ownerName)
    {
        var affected = orderedMappings
            .Where(m => attr.ExceptionType.IsAssignableFrom(m.ResolvedType))
            .ToList();

        if (affected.Count == 0)
            throw new InvalidOperationException(
                $"[ExceptionDisplay] on '{ownerName}' references exception type '{attr.ExceptionType.Name}' " +
                $"which has no matching or derived entry in exceptionMappings. Add it to error-mapping.json.");

        for (var i = 0; i < orderedMappings.Count; i++)
        {
            if (affected.Contains(orderedMappings[i]))
                orderedMappings[i] = orderedMappings[i] with { DisplayMode = attr.DisplayMode };
        }
    }
}
