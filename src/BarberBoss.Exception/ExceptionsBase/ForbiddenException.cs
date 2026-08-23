using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;
public class ForbiddenException : BarberBossException
{
    public ForbiddenException(string message) : base(message)
    {
    }

    public override int StatusCode => (int)HttpStatusCode.Forbidden;

    public override List<string> GetErrors()
    {
        return [Message];
    }
}
