using System.Reflection;
using BFFConductor.Attributes;
using BFFConductor.Configuration;
using BFFConductor.Interfaces;
using BFFConductor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BFFConductor.Filters;

public class BffResponseFilter : IActionFilter
{
    private const string HeaderName = "x-handle-message-as";

    private readonly ErrorMappingRegistry _registry;
    private readonly BffResponseOptions _options;

    public BffResponseFilter(ErrorMappingRegistry registry, BffResponseOptions options)
    {
        _registry = registry;
        _options = options;
    }

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null) return;
        if (context.Result is not ObjectResult { Value: IOperationResult operationResult }) return;

        var resolvedMap = BuildResolvedMap(context);

        if (operationResult.Success)
        {
            var displayMethod = operationResult.DisplayMethod ?? _registry.DefaultDisplayMethod;

            context.HttpContext.Response.Headers[HeaderName] = displayMethod;
            context.Result = new OkObjectResult(new ApiResponse<object?>
            {
                Success = true,
                Data = operationResult.GetData(),
                Errors = []
            });
        }
        else
        {
            var firstErrorCode = operationResult.Errors.FirstOrDefault()?.Code;

            ErrorMapping? mapping = null;
            if (firstErrorCode is not null && resolvedMap.TryGetValue(firstErrorCode, out var mapped))
                mapping = mapped;

            var displayMethod = mapping?.DisplayMethod ?? _registry.DefaultDisplayMethod;
            var httpStatus = mapping?.HttpStatus ?? 500;

            context.HttpContext.Response.Headers[HeaderName] = displayMethod;

            if (mapping?.AdditionalHeaders is not null)
            {
                foreach (var (key, value) in mapping.AdditionalHeaders)
                    context.HttpContext.Response.Headers[key] = value;
            }

            context.Result = new ObjectResult(new ApiResponse<object?>
            {
                Success = false,
                Data = null,
                Errors = operationResult.Errors
                    .Select(e => new ApiError { Message = e.Message, Code = e.Code })
                    .ToList()
            })
            { StatusCode = httpStatus };
        }
    }

    /// <summary>
    /// Builds the effective error-code → mapping dictionary for this request.
    /// Resolution order: global spec → controller attributes → action attributes (most specific wins).
    /// Attributes only override DisplayMethod; HttpStatus always comes from the spec.
    /// </summary>
    private Dictionary<string, ErrorMapping> BuildResolvedMap(ActionExecutedContext context)
    {
        var map = _registry.CloneMappings();

        // Controller-level [ErrorDisplay] attributes
        var controllerAttrs = context.Controller.GetType()
            .GetCustomAttributes<ErrorDisplayAttribute>(inherit: true);

        foreach (var attr in controllerAttrs)
        {
            if (!map.TryGetValue(attr.ErrorCode, out var existing))
                throw new InvalidOperationException(
                    $"[ErrorDisplay] on '{context.Controller.GetType().Name}' references error code '{attr.ErrorCode}' which has no entry in the mapping spec. Add it to error-mapping.json.");

            map[attr.ErrorCode] = existing with { DisplayMethod = attr.DisplayMethod };
        }

        // Action-level [ErrorDisplay] attributes — most specific wins
        if (context.ActionDescriptor is ControllerActionDescriptor cad)
        {
            var actionAttrs = cad.MethodInfo.GetCustomAttributes<ErrorDisplayAttribute>(inherit: false);

            foreach (var attr in actionAttrs)
            {
                if (!map.TryGetValue(attr.ErrorCode, out var existing))
                    throw new InvalidOperationException(
                        $"[ErrorDisplay] on '{cad.MethodInfo.Name}' references error code '{attr.ErrorCode}' which has no entry in the mapping spec. Add it to error-mapping.json.");

                map[attr.ErrorCode] = existing with { DisplayMethod = attr.DisplayMethod };
            }
        }

        return map;
    }
}
