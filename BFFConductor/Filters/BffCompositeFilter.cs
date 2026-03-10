using Microsoft.AspNetCore.Mvc.Filters;

namespace BFFConductor.Filters;

internal class BffCompositeFilter : IExceptionFilter
{
    private readonly BffExceptionFilter _exceptionFilter;

    public BffCompositeFilter(BffExceptionFilter exceptionFilter)
    {
        _exceptionFilter = exceptionFilter;
    }

    public void OnException(ExceptionContext context) =>
        _exceptionFilter.OnException(context);
}
