namespace BFFConductor.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ErrorDisplayAttribute : Attribute
{
    public string ErrorCode { get; }
    public string DisplayMode { get; }

    public ErrorDisplayAttribute(string errorCode, string displayMode)
    {
        ErrorCode = errorCode;
        DisplayMode = displayMode;
    }
}
