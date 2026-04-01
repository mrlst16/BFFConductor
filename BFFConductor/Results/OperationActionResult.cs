using System.Reflection;
using BFFConductor.Attributes;
using BFFConductor.Configuration;
using BFFConductor.Interfaces;
using BFFConductor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace BFFConductor.Results;

public class OperationActionResult<T> : IActionResult
{
    private const string HeaderName = "x-handle-message-as";

    private readonly IOperationResult _operationResult;

    public OperationActionResult(OperationResult<T> operationResult)
    {
        _operationResult = operationResult;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var _registry = context.HttpContext.RequestServices.GetRequiredService<ErrorMappingRegistry>();
        var resolvedMap = BuildResolvedMap(context, _registry);
        ObjectResult objectResult;

        if (_operationResult.Success)
        {
            var displayMode = _operationResult.DisplayMode ?? _registry.DefaultDisplayMode;

            context.HttpContext.Response.Headers[HeaderName] = displayMode;
            objectResult = new OkObjectResult(new ApiResponse<object?>
            {
                Success = true,
                Data = _operationResult.GetData(),
                Errors = []
            });
        }
        else
        {
            var firstErrorCode = _operationResult.Errors.FirstOrDefault()?.Code;

            ErrorMapping? mapping = null;
            if (firstErrorCode is not null && resolvedMap.TryGetValue(firstErrorCode, out var mapped))
                mapping = mapped;

            var displayMode = mapping?.DisplayMode ?? _registry.DefaultDisplayMode;
            var httpStatus = mapping?.HttpStatus ?? 500;

            context.HttpContext.Response.Headers[HeaderName] = displayMode;

            if (mapping?.AdditionalHeaders is not null)
            {
                foreach (var (key, value) in mapping.AdditionalHeaders)
                    context.HttpContext.Response.Headers[key] = value;

                // Expose additional headers so JavaScript can read them across origins
                var currentExposed = context.HttpContext.Response.Headers["Access-Control-Expose-Headers"].ToString();
                var toAppend = string.Join(", ", mapping.AdditionalHeaders.Keys);
                context.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = string.IsNullOrEmpty(currentExposed)
                    ? toAppend
                    : currentExposed + ", " + toAppend;
            }

            objectResult = new ObjectResult(new ApiResponse<object?>
            {
                Success = false,
                Data = null,
                Errors = _operationResult.Errors
                    .Select(e => new ApiError { Message = e.Message, Code = e.Code })
                    .ToList()
            })
            { StatusCode = httpStatus };
        }

        // Delegate to ObjectResult to handle actual response serialization
        await objectResult.ExecuteResultAsync(context);
    }

    /// <summary>
    /// Builds the effective error-code → mapping dictionary for this request.
    /// Resolution order: global spec → controller attributes → action attributes (most specific wins).
    /// Attributes only override DisplayMode; HttpStatus always comes from the spec.
    /// </summary>
    private Dictionary<string, ErrorMapping> BuildResolvedMap(ActionContext context, ErrorMappingRegistry registry)
    {
        var map = registry.CloneMappings();

        // Controller-level [ErrorDisplay] attributes
        if (context.ActionDescriptor is ControllerActionDescriptor cad)
        {
            var controllerAttrs = cad.ControllerTypeInfo.GetCustomAttributes<ErrorDisplayAttribute>(inherit: true);

            foreach (var attr in controllerAttrs)
            {
                if (!map.TryGetValue(attr.ErrorCode, out var existing))
                    throw new InvalidOperationException(
                        $"[ErrorDisplay] on '{cad.ControllerTypeInfo.Name}' references error code '{attr.ErrorCode}' which has no entry in the mapping spec. Add it to error-mapping.json.");

                map[attr.ErrorCode] = existing with { DisplayMode = attr.DisplayMode };
            }

            // Action-level [ErrorDisplay] attributes — most specific wins
            var actionAttrs = cad.MethodInfo.GetCustomAttributes<ErrorDisplayAttribute>(inherit: false);

            foreach (var attr in actionAttrs)
            {
                if (!map.TryGetValue(attr.ErrorCode, out var existing))
                    throw new InvalidOperationException(
                        $"[ErrorDisplay] on '{cad.MethodInfo.Name}' references error code '{attr.ErrorCode}' which has no entry in the mapping spec. Add it to error-mapping.json.");

                map[attr.ErrorCode] = existing with { DisplayMode = attr.DisplayMode };
            }
        }

        return map;
    }
}
