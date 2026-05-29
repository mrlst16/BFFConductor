using Microsoft.AspNetCore.Mvc.Filters;

namespace BFFConductor.Filters;

internal class BffCompositeFilter : IAsyncExceptionFilter
{
    private readonly BffExceptionFilter _exceptionFilter;

    public BffCompositeFilter(BffExceptionFilter exceptionFilter)
    {
        _exceptionFilter = exceptionFilter;
    }

    public Task OnExceptionAsync(ExceptionContext context) =>
        _exceptionFilter.OnExceptionAsync(context);
}
