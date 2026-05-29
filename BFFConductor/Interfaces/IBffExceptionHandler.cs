namespace BFFConductor.Interfaces;

public interface IBffExceptionHandler
{
    Task HandleAsync(Exception exception);
}
