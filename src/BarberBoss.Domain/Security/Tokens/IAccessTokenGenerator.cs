using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Security.Tokens;
public interface IAccessTokenGenerator
{
    string Generate(User user);
}
