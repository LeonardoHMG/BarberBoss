using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;

public class ConflictException : BarberBossException
{
    public ConflictException(string message) : base(message)
    {  
    }

    public override int StatusCode => (int)HttpStatusCode.Conflict;

    public override List<string> GetErros()
    {
        return [Message];
    }
}