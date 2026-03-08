using BFFConductor.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BFFConductor.Attributes;

public class UseBFFResponseFilterAttribute : TypeFilterAttribute
{
    public UseBFFResponseFilterAttribute() : base(typeof(BffResponseFilter)) { }
}
