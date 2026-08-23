using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Services.LoggedUser;
public interface ILoggedUser
{
    Task<User> Get();
}
