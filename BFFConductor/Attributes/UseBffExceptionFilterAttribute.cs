using BFFConductor.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BFFConductor.Attributes;

public class UseBffExceptionFilterAttribute : TypeFilterAttribute
{
    public UseBffExceptionFilterAttribute() : base(typeof(BffCompositeFilter)) { }
}
