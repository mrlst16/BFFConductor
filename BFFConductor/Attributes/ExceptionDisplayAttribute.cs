namespace BFFConductor.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ExceptionDisplayAttribute : Attribute
{
    public Type ExceptionType { get; }
    public string DisplayMode { get; }

    public ExceptionDisplayAttribute(Type exceptionType, string displayMode)
    {
        ExceptionType = exceptionType;
        DisplayMode = displayMode;
    }
}
