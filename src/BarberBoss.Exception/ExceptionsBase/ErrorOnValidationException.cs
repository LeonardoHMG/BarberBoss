using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;
public class ErrorOnValidationException : BarberBossException
{
    private readonly List<string> _errors;

    public override int StatusCode => (int)HttpStatusCode.BadRequest;

    public ErrorOnValidationException(List<string> errorMessages) : base(string.Empty) 
    {
        _errors = errorMessages; 
    }

    public override List<string> GetErrors()
    {
        return _errors;
    }
}
