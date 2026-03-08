namespace BFFConductor.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ErrorDisplayAttribute : Attribute
{
    public string ErrorCode { get; }
    public string DisplayMethod { get; }

    public ErrorDisplayAttribute(string errorCode, string displayMethod)
    {
        ErrorCode = errorCode;
        DisplayMethod = displayMethod;
    }
}
