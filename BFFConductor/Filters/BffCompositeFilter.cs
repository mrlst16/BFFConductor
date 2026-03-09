using Microsoft.AspNetCore.Mvc.Filters;

namespace BFFConductor.Filters;

internal class BffCompositeFilter : IActionFilter, IExceptionFilter
{
    private readonly BffResponseFilter _responseFilter;
    private readonly BffExceptionFilter _exceptionFilter;

    public BffCompositeFilter(BffResponseFilter responseFilter, BffExceptionFilter exceptionFilter)
    {
        _responseFilter = responseFilter;
        _exceptionFilter = exceptionFilter;
    }

    public void OnActionExecuting(ActionExecutingContext context) =>
        _responseFilter.OnActionExecuting(context);

    public void OnActionExecuted(ActionExecutedContext context) =>
        _responseFilter.OnActionExecuted(context);

    public void OnException(ExceptionContext context) =>
        _exceptionFilter.OnException(context);
}
