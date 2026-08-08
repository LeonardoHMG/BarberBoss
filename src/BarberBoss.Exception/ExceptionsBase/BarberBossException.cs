namespace BarberBoss.Exception.ExceptionsBase;
public abstract class BarberBossException : SystemException
{
    protected BarberBossException(string message) : base(message)
    {
    }

    public abstract int StatusCode { get; }
    public abstract List<string> GetErrors();
}

