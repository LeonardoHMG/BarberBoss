namespace BarberBoss.Domain.Security.Tokens;
public interface ITokenProvider
{
    string TokenOnRequest();
}
